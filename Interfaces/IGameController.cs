using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Interfaces
{
	interface IGameController
	{
		ActionResult<string> GetGameRules();
		ActionResult<string> GetUserSubmission(string userName, string antName);
		ActionResult SubmitUserEntry(string userName, string antName, string submission);
		ActionResult DeleteSubmission(string userName, string antName);
	}
}
