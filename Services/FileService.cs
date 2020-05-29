using CodeChallengeInc.SubmissionApi.Constants;
using CodeChallengeInc.SubmissionApi.Interfaces;
using CodeChallengeInc.SubmissionApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Services
{
	public class FileService : IFileService
	{
		public void BackupUserSubmission(string path)
		{
			string content = File.ReadAllText(path);
			FileInfo fi = new FileInfo(path);

			string filename = $"{fi.Name.Replace(FileInformation.LoneAntFileExtension, string.Empty)} - {fi.LastWriteTime.ToString(FileInformation.DateTimeFormat)}{fi.Extension}";
			string backupLocation = Path.Combine(GetBackupsPath(), filename);

			File.WriteAllText(backupLocation, content);
		}

		public void CreateOrOverwriteUserSubmission(string username, string submission)
		{
			if (UserSubmissionExists(username))
			{
				BackupUserSubmission(GetUserSubmissionPath(username));
			}

			using (FileStream fs = System.IO.File.Create(GetUserSubmissionPath(username)))
			{
				byte[] content = new UTF8Encoding(true).GetBytes(submission);

				fs.Write(content, 0, content.Length);
			}
		}

		public void DeleteUserSubmission(string username)
		{
			BackupUserSubmission(GetUserSubmissionPath(username));
			File.Delete(GetUserSubmissionPath(username));
		}

		public bool UserSubmissionExists(string username)
		{
			return File.Exists(GetUserSubmissionPath(username));
		}

		public LoneAntSubmission GetUserSubmission(string username)
		{
			return new LoneAntSubmission { Username = username, Submission = File.ReadAllText(GetUserSubmissionPath(username)) };
		}
		internal string GetSubmissionsPath()
		{
			return Path.Combine(FileInformation.LoneAntFolder, FileInformation.SubmissionFolder);
		}

		internal string GetBackupsPath()
		{
			return Path.Combine(FileInformation.LoneAntFolder, FileInformation.BackupSubmissionFolder);
		}

		internal string GetUserSubmissionPath(string username)
		{
			return Path.Combine(GetSubmissionsPath(), username + FileInformation.LoneAntFileExtension);
		}

		public List<LoneAntSubmission> GetSubmissionsJson()
		{
			List<LoneAntSubmission> submissions = new List<LoneAntSubmission>();
			foreach(string submissionName in GetSubmissionNames())
			{
				submissions.Add(GetUserSubmission(submissionName));
			}

			return submissions;
		}

		public List<string> GetSubmissionNames()
		{
			List<string> submissionNames = new List<string>();
			foreach (string submissionPath in Directory.EnumerateFiles(GetSubmissionsPath()))
			{
				string submissionName= submissionPath.Replace($"{GetSubmissionsPath()}\\", string.Empty);
				submissionNames.Add(submissionName.Replace(FileInformation.LoneAntFileExtension, string.Empty));
			}

			return submissionNames;
		}
	}
}
