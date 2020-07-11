using CodeChallengeInc.SubmissionApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Interfaces
{
	public interface IFileService
	{
		bool UserSubmissionExists(string gameType, string name);
		LoneAntSubmissionResponse GetUserSubmission(string gameType, string name, string userName);
		void CreateOrOverwriteUserSubmission(string gameType, string name, string userName, string submission);
		void DeleteUserSubmission(string gameType, string name, string userName);
		void BackupUserSubmission(string gameType, string path);
		List<LoneAntSubmissionResponse> GetSubmissionsJson(string gameType);
		List<string> GetSubmissionNames(string gameType);
		void PurgeDefaultAnts(string gameType);

	}
}
