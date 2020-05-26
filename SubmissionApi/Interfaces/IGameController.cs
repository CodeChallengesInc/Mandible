using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubmissionApi.Interfaces
{
	interface IGameController
	{
		ActionResult<string> GetGameRules();
		ActionResult<string> GetUserSubmission(string username);
		void SubmitUserEntry(string username, [FromBody] string submission);
		void DeleteSubmission(string username);
	}
}
