using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Interfaces
{
	interface IGameController
	{
		ActionResult<string> GetGameRules(string gameType);
		ActionResult<string> GetUserSubmission(string gameType, string userName, string name);
		ActionResult SubmitUserEntry(string gameType, string userName, string name, string submission);
		ActionResult DeleteSubmission(string gameType, string userName, string name);
	}
}
