using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CodeChallengeInc.SubmissionApi.Interfaces;
using CodeChallengeInc.SubmissionApi.Constants;
using System.IO;
using System;
using Newtonsoft.Json;
using CodeChallengeInc.SubmissionApi.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;

namespace CodeChallengeInc.SubmissionApi.Controllers
{
	[Route("lone-ant")]
	[ApiController]
	public class LoneAntController : ControllerBase//, IGameController
	{
		internal string Passcode { get; set; }
		IFileService _fileService;
		public LoneAntController(IFileService fileService)
		{
			_fileService = fileService;
			Passcode = (Environment.GetEnvironmentVariable(EnvironmentConstants.PasscodeVariable) != null) ? Environment.GetEnvironmentVariable(EnvironmentConstants.PasscodeVariable) : EnvironmentConstants.DefaultPasscode;
		}
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<string> GetGameRules()
		{
			return GameRules.LoneAnt;
		}

		[HttpGet("{username}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<string> GetUserSubmission(string username)
		{
			if (_fileService.UserSubmissionExists(username))
			{
				return _fileService.GetUserSubmissionJson(username);
			}
			return NotFound(JsonConvert.SerializeObject(new ErrorResponse { ErrorCode = 404, ErrorMessage = ErrorResponses.SubmissionNotFound(username) }));
		}

		[HttpPut("{username}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult SubmitUserEntry(string username, [FromBody] string submission)
		{
			if (submission != string.Empty)
			{
				_fileService.CreateOrOverwriteUserSubmission(username, submission);
				return NoContent();
			}
			return BadRequest(JsonConvert.SerializeObject(new ErrorResponse { ErrorCode = 400, ErrorMessage = ErrorResponses.SubmissionPutBodyEmpty }));
		}

		[HttpDelete("{username}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteSubmission(string username)
		{
			if (_fileService.UserSubmissionExists(username))
			{
				_fileService.DeleteUserSubmission(username);
				return NoContent();
			}
			return NotFound(JsonConvert.SerializeObject(new ErrorResponse { ErrorCode = 404, ErrorMessage = ErrorResponses.SubmissionNotFound(username) }));
		}
	}
}
