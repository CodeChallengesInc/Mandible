using CodeChallengeInc.SubmissionApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Interfaces
{
	public interface IFileService
	{
		bool UserSubmissionExists(string antName);
		LoneAntSubmissionResponse GetUserSubmission(string antName, string userName);
		void CreateOrOverwriteUserSubmission(string antName, string userName, string submission);
		void DeleteUserSubmission(string antName, string userName);
		void BackupUserSubmission(string path);
		List<LoneAntSubmissionResponse> GetSubmissionsJson();
		List<string> GetSubmissionNames();

	}
}
