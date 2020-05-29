using CodeChallengeInc.SubmissionApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Interfaces
{
	public interface IFileService
	{
		bool UserSubmissionExists(string username);
		LoneAntSubmission GetUserSubmission(string username);
		void CreateOrOverwriteUserSubmission(string username, string submission);
		void DeleteUserSubmission(string username);
		void BackupUserSubmission(string path);
		List<LoneAntSubmission> GetSubmissionsJson();
		List<string> GetSubmissionNames();

	}
}
