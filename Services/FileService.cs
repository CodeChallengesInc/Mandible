using CodeChallengeInc.SubmissionApi.Constants;
using CodeChallengeInc.SubmissionApi.Interfaces;
using CodeChallengeInc.SubmissionApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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

        public void CreateOrOverwriteUserSubmission(string userName, string antName, string submission)
        {
            userName = userName.Replace("_", string.Empty);
            string fileName = $"{userName}_{antName}";
            string userSubmissionPath = GetUserSubmissionPath(fileName);
            if (UserSubmissionExists(userName, antName))
            {
                BackupUserSubmission(userSubmissionPath);
            }

            using (FileStream fs = File.Create(userSubmissionPath))
            {
                byte[] content = new UTF8Encoding(true).GetBytes(submission);
                fs.Write(content, 0, content.Length);
            }
        }

        public void DeleteUserSubmission(string userName, string antName)
        {
            string fileName = $"{userName}_{antName}";
            string userSubmissionPath = GetUserSubmissionPath(fileName);
            BackupUserSubmission(userSubmissionPath);
            File.Delete(userSubmissionPath);
        }

        public bool UserSubmissionExists(string userName, string antName)
		{
            string fileName = $"{userName}_{antName}";
            string userSubmissionPath = GetUserSubmissionPath(fileName);
            return File.Exists(userSubmissionPath);
        }

		public LoneAntSubmissionResponse GetUserSubmission(string userName, string antName)
		{
			string filePath = GetUserSubmissionPath($"{userName}_{antName}");
			return new LoneAntSubmissionResponse { Username = userName, Submission = File.ReadAllText(filePath), AntName = antName };
		}
		internal string GetSubmissionsPath()
		{
			return Path.Combine(FileInformation.LoneAntFolder, FileInformation.SubmissionFolder);
		}

		internal string GetBackupsPath()
		{
			return Path.Combine(FileInformation.LoneAntFolder, FileInformation.BackupSubmissionFolder);
		}

        internal string GetUserSubmissionPath(string fileName)
        {
            string submissionsPath = GetSubmissionsPath();
            if (fileName.EndsWith(FileInformation.LoneAntFileExtension)) return Path.Combine(submissionsPath, fileName);
            return Path.Combine(submissionsPath, fileName + FileInformation.LoneAntFileExtension);
        }

        public List<LoneAntSubmissionResponse> GetSubmissionsJson()
		{
			List<LoneAntSubmissionResponse> submissions = new List<LoneAntSubmissionResponse>();
			foreach(string submissionName in GetSubmissionNames())
			{
				ExtractUsernameAndAntNameFromSubmissionName(submissionName, out string userName, out string antName);
				submissions.Add(GetUserSubmission(userName, antName));
			}

			return submissions;
		}

		public List<string> GetSubmissionNames()
		{
			List<string> submissionNames = new List<string>();
			
			foreach (string submissionPath in Directory.EnumerateFiles(GetSubmissionsPath()))
			{
				string submissionName = ExtractSubmissionNameFromPath(submissionPath);
				
				submissionNames.Add(submissionName.Replace(FileInformation.LoneAntFileExtension, string.Empty));
			}

			return submissionNames;
		}

		public void PurgeDefaultAnts()
		{
			foreach(string submissionPath in Directory.EnumerateFiles(GetSubmissionsPath()))
			{
				string submissionName = ExtractSubmissionNameFromPath(submissionPath);
				ExtractUsernameAndAntNameFromSubmissionName(submissionName, out string userName, out string antName);
				string submissionText = GetUserSubmission(userName, antName).Submission;
				if (submissionText.Equals(FileInformation.DefaultAntString))
				{
					DeleteUserSubmission(antName, userName);
				}
			}
		}

		internal string ExtractSubmissionNameFromPathForWindows(string path)
		{
			return path.Replace($"{GetSubmissionsPath()}\\", string.Empty);
		}

		internal string ExtractSubmissionNameFromPathForLinux(string path)
		{
			return path.Replace(FileInformation.DockerFilePathPrefix, string.Empty);
		}

		internal string ExtractSubmissionNameFromPath(string path)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return ExtractSubmissionNameFromPathForWindows(path);
			}
			else
			{
				return ExtractSubmissionNameFromPathForLinux(path);
			}
		}

		internal void ExtractUsernameAndAntNameFromSubmissionName(string submissionName, out string userName, out string antName)
		{
			List<string> fileNameParts = submissionName.Split('_').ToList();
			userName = fileNameParts[0];
			fileNameParts.RemoveAt(0);
            antName = string.Join("_", fileNameParts);
        }
	}
}
