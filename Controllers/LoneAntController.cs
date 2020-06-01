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
	public class LoneAntController : ControllerBase, IGameController
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

		[HttpGet("{userName}/{antName}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult<string> GetUserSubmission(string userName, string antName)
		{
			string fileName = $"{userName}_{antName}";
			if (_fileService.UserSubmissionExists(fileName))
			{
				return Ok(_fileService.GetUserSubmission(antName, userName));
			}
			return NotFound(new ErrorResponse { ErrorCode = 404, ErrorMessage = ErrorResponses.SubmissionNotFound($"{userName}_{antName}") });
		}

		[HttpPut("{userName}/{antName}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[Produces("application/json")]
		public ActionResult SubmitUserEntry(string userName, string antName, [FromBody] string submission)
		{
			if (submission != null)
			{
				_fileService.CreateOrOverwriteUserSubmission(antName, userName, submission);
				return NoContent();
			}
			return BadRequest(new ErrorResponse { ErrorCode = 400, ErrorMessage = ErrorResponses.SubmissionPutBodyEmpty });
		}

		[HttpDelete("{userName}/{antName}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult DeleteSubmission(string antName, string userName)
		{
			if (_fileService.UserSubmissionExists(antName))
			{
				_fileService.DeleteUserSubmission(antName, userName);
				return NoContent();
			}
			return NotFound(new ErrorResponse { ErrorCode = 404, ErrorMessage = ErrorResponses.SubmissionNotFound($"{userName}_{antName}") });
		}

		[HttpGet("submissions")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult<string> GetUserSubmissions()
		{
			List<LoneAntSubmission> userSubmissions = _fileService.GetSubmissionsJson();
			if(userSubmissions.Count == 0)
			{
				return NoContent();
			}
			return Ok(userSubmissions);
		}

		[HttpGet("submission-names")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult<string> GetUserSubmissionNames()
		{
			List<string> userSubmissionNames = _fileService.GetSubmissionNames();
			if (userSubmissionNames.Count == 0)
			{
				return NoContent();
			}
			return Ok(userSubmissionNames);
		}
	}
}
