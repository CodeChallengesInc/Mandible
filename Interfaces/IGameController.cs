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
		ActionResult<string> GetUserSubmission(string username);
		ActionResult SubmitUserEntry(string username);
		ActionResult DeleteSubmission(string username);
	}
}
