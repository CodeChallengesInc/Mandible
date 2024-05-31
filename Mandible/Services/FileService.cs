using CodeChallengeInc.Mandible.Constants;
using CodeChallengeInc.Mandible.Interfaces;
using CodeChallengeInc.Mandible.Models;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeChallengeInc.Mandible.Services
{
	public class FileService : IFileService
	{
        private readonly IFileSystem _fileSystem;
        public FileService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        public void BackupUserSubmission(string path)
        {
            string content = _fileSystem.File.ReadAllText(path);
            FileInfoBase fi = new FileInfo(path);

            string filename = $"{fi.Name.Replace(FileInformation.LoneAntFileExtension, string.Empty)} - {fi.LastWriteTime.ToString(FileInformation.DateTimeFormat)}{fi.Extension}";
            string backupLocation = _fileSystem.Path.Combine(GetBackupsPath(), filename);

            _fileSystem.File.WriteAllText(backupLocation, content);
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

            using (Stream fs = _fileSystem.File.Create(userSubmissionPath))
            {
                byte[] content = new UTF8Encoding(true).GetBytes(submission);
                fs.Write(content, 0, content.Length);
            }
        }

        public void DeleteUserSubmission(string userName, string antName)
        {
			if (UserSubmissionExists(userName, antName))
			{
				string fileName = $"{userName}_{antName}";
                string userSubmissionPath = GetUserSubmissionPath(fileName);
                BackupUserSubmission(userSubmissionPath);
                _fileSystem.File.Delete(userSubmissionPath);
            }
        }

        public bool UserSubmissionExists(string userName, string antName)
		{
            string fileName = $"{userName}_{antName}";
            string userSubmissionPath = GetUserSubmissionPath(fileName);
			Console.WriteLine($"Validating if {userSubmissionPath} exists");
            return _fileSystem.File.Exists(userSubmissionPath);
        }

		public LoneAntSubmissionResponse GetUserSubmission(string userName, string antName)
		{
			string filePath = GetUserSubmissionPath($"{userName}_{antName}");
			string submission = _fileSystem.File.ReadAllText(filePath);
			return new LoneAntSubmissionResponse { Username = userName, Submission = submission, AntName = antName };
		}
		internal string GetSubmissionsPath()
		{
			return _fileSystem.Path.Combine(FileInformation.LoneAntFolder, FileInformation.SubmissionFolder);
		}

		internal string GetBackupsPath()
		{
			return _fileSystem.Path.Combine(FileInformation.LoneAntFolder, FileInformation.BackupSubmissionFolder);
		}

        internal string GetUserSubmissionPath(string fileName)
        {
            string submissionsPath = GetSubmissionsPath();
            if (fileName.EndsWith(FileInformation.LoneAntFileExtension)) return _fileSystem.Path.Combine(submissionsPath, fileName);
            return _fileSystem.Path.Combine(submissionsPath, fileName + FileInformation.LoneAntFileExtension);
        }

        public List<LoneAntSubmissionResponse> GetSubmissionsReponse()
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
			
			foreach (string submissionPath in _fileSystem.Directory.EnumerateFiles(GetSubmissionsPath()))
			{
				string submissionName = ExtractSubmissionNameFromPath(submissionPath);
				
				submissionNames.Add(submissionName.Replace(FileInformation.LoneAntFileExtension, string.Empty));
			}

			return submissionNames;
		}

		public void PurgeDefaultAnts()
		{
			string submissionsPath = GetSubmissionsPath();
			var shuckyduck = _fileSystem.Directory.EnumerateFiles(submissionsPath);

            foreach (string submissionPath in shuckyduck)
			{
				string submissionName = ExtractSubmissionNameFromPath(submissionPath);
				ExtractUsernameAndAntNameFromSubmissionName(submissionName, out string userName, out string antName);
				string submissionText = GetUserSubmission(userName, antName).Submission;
				if (submissionText.Equals(FileInformation.DefaultAntString))
				{
					DeleteUserSubmission(userName, antName);
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
