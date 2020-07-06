using CodeChallengeInc.SubmissionApi.Constants;
using CodeChallengeInc.SubmissionApi.Interfaces;
using CodeChallengeInc.SubmissionApi.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Services
{
	public class FileService : IFileService
	{
		public void BackupUserSubmission(string gameType, string path)
		{
			string content = File.ReadAllText(path);
			FileInfo fi = new FileInfo(path);

			string filename = $"{fi.Name.Replace(FileInformation.LoneAntFileExtension, string.Empty)} - {fi.LastWriteTime.ToString(FileInformation.DateTimeFormat)}{fi.Extension}";
			string backupLocation = Path.Combine(GetBackupsPath(gameType), filename);

			File.WriteAllText(backupLocation, content);
		}

		public void CreateOrOverwriteUserSubmission(string gameType, string antName, string userName, string submission)
		{
			userName = userName.Replace("_", string.Empty);
			string fileName = $"{userName}_{antName}"; 
			if (UserSubmissionExists(gameType, fileName))
			{
				BackupUserSubmission(gameType, GetUserSubmissionPath(gameType, fileName));
			}

			using (FileStream fs = File.Create(GetUserSubmissionPath(gameType, fileName)))
			{
				byte[] content = new UTF8Encoding(true).GetBytes(submission);

				fs.Write(content, 0, content.Length);
			}
		}

		public void DeleteUserSubmission(string gameType, string antName, string userName)
		{
			string fileName = $"{userName}_{antName}";
			BackupUserSubmission(gameType, GetUserSubmissionPath(gameType, fileName));
			File.Delete(GetUserSubmissionPath(gameType, fileName));
		}

		public bool UserSubmissionExists(string gameType, string fileName)
		{
			return File.Exists(GetUserSubmissionPath(gameType, fileName));
		}

		public LoneAntSubmissionResponse GetUserSubmission(string gameType, string antName, string userName)
		{
			string filePath = GetUserSubmissionPath(gameType, $"{userName}_{antName}");
			return new LoneAntSubmissionResponse { Username = userName, Submission = File.ReadAllText(filePath), AnimalName = antName };
		}
		internal string GetSubmissionsPath(string gameType)
		{
			return Path.Combine(FileInformation.SubmissionFolder, gameType);
		}

		internal string GetBackupsPath(string gameType)
		{
			return Path.Combine(FileInformation.BackupSubmissionFolder, gameType);
		}

		internal string GetUserSubmissionPath(string gameType, string fileName)
		{
			if (fileName.Contains(FileInformation.LoneAntFileExtension)) return Path.Combine(GetSubmissionsPath(gameType), fileName);
			return Path.Combine(GetSubmissionsPath(gameType), fileName + FileInformation.LoneAntFileExtension);
		}

		public List<LoneAntSubmissionResponse> GetSubmissionsJson(string gameType)
		{
			List<LoneAntSubmissionResponse> submissions = new List<LoneAntSubmissionResponse>();
			foreach(string submissionName in GetSubmissionNames(gameType))
			{
				ExtractUsernameAndAntNameFromSubmissionName(submissionName, out string userName, out string antName);
				submissions.Add(GetUserSubmission(gameType, antName, userName));
			}

			return submissions;
		}

		public List<string> GetSubmissionNames(string gameType)
		{
			List<string> submissionNames = new List<string>();
			
			foreach (string submissionPath in Directory.EnumerateFiles(GetSubmissionsPath(gameType)))
			{
				string submissionName = ExtractSubmissionNameFromPath(gameType, submissionPath);
				
				submissionNames.Add(submissionName.Replace(FileInformation.LoneAntFileExtension, string.Empty));
			}

			return submissionNames;
		}

		public void PurgeDefaultAnts(string gameType)
		{
			foreach(string submissionPath in Directory.EnumerateFiles(GetSubmissionsPath(gameType)))
			{
				string submissionName = ExtractSubmissionNameFromPath(gameType, submissionPath);
				ExtractUsernameAndAntNameFromSubmissionName(submissionName, out string userName, out string antName);
				string submissionText = GetUserSubmission(gameType, antName, userName).Submission;
				if (submissionText.Equals(FileInformation.DefaultAntString))
				{
					DeleteUserSubmission(gameType, antName, userName);
				}
			}
		}

		internal string ExtractSubmissionNameFromPathForWindows(string gameType, string path)
		{
			return path.Replace($"{GetSubmissionsPath(gameType)}\\", string.Empty);
		}

		internal string ExtractSubmissionNameFromPathForLinux(string gameType, string path)
		{
			return path.Replace(FileInformation.DockerFilePathPrefix, string.Empty).Replace($"{gameType}/", string.Empty);
		}

		internal string ExtractSubmissionNameFromPath(string gameType, string path)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return ExtractSubmissionNameFromPathForWindows(gameType, path);
			}
			else
			{
				return ExtractSubmissionNameFromPathForLinux(gameType, path);
			}
		}

		internal void ExtractUsernameAndAntNameFromSubmissionName(string submissionName, out string userName, out string antName)
		{
			List<string> fileNameParts = submissionName.Split('_').ToList();
			userName = fileNameParts[0];
			fileNameParts.RemoveAt(0);
			antName = fileNameParts.Join("_");
		}
	}
}
