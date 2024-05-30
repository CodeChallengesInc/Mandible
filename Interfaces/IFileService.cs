using CodeChallengeInc.SubmissionApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Interfaces
{
	public interface IFileService
	{
		bool UserSubmissionExists(string userName, string antName);
		LoneAntSubmissionResponse GetUserSubmission(string userName, string antName);
		void CreateOrOverwriteUserSubmission(string userName, string antName, string submission);
		void DeleteUserSubmission(string userName, string antName);
		void BackupUserSubmission(string path);
		List<LoneAntSubmissionResponse> GetSubmissionsJson();
		List<string> GetSubmissionNames();
		void PurgeDefaultAnts();

	}
}
