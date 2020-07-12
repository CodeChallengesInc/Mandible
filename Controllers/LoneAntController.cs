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
	[ApiController]
	public class GameSubmissionController : ControllerBase, IGameController
	{
		internal string Passcode { get; set; }
		IFileService _fileService;
		public GameSubmissionController(IFileService fileService)
		{
			_fileService = fileService;
			Passcode = (Environment.GetEnvironmentVariable(EnvironmentConstants.PasscodeVariable) != null) ? Environment.GetEnvironmentVariable(EnvironmentConstants.PasscodeVariable) : EnvironmentConstants.DefaultPasscode;
		}
		[HttpGet("{gameType}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<string> GetGameRules(string gameType)
		{
			return GameRules.LoneAnt;
		}

		[HttpGet("{gameType}/{userName}/{name}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult<string> GetUserSubmission(string gameType, string userName, string name)
		{
			string fileName = $"{userName}_{name}";
			if (_fileService.UserSubmissionExists(gameType, fileName))
			{
				return Ok(_fileService.GetUserSubmission(gameType, name, userName));
			}
			return NotFound(new ErrorResponse { ErrorCode = 404, ErrorMessage = ErrorResponses.SubmissionNotFound($"{gameType}/{userName}_{name}") });
		}

		[HttpPut("{gameType}/{userName}/{name}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[Produces("application/json")]
		public ActionResult SubmitUserEntry(string gameType, string userName, string name, [FromBody] string submission)
		{
			if (submission != null)
			{
				string decodedString = string.Empty;
				if(!string.IsNullOrWhiteSpace(submission))
				{
					byte[] data = Convert.FromBase64String(submission);
					decodedString = Encoding.UTF8.GetString(data);	
				}
				_fileService.CreateOrOverwriteUserSubmission(gameType, name, userName, decodedString);
				return NoContent();
			}
			return BadRequest(new ErrorResponse { ErrorCode = 400, ErrorMessage = ErrorResponses.SubmissionPutBodyEmpty });
		}

		[HttpDelete("{gameType}/{userName}/{name}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult DeleteSubmission(string gameType, string name, string userName)
		{
			string fileName = $"{userName}_{name}";
			if (_fileService.UserSubmissionExists(gameType, fileName))
			{
				_fileService.DeleteUserSubmission(gameType, name, userName);
				return NoContent();
			}
			return NotFound(new ErrorResponse { ErrorCode = 404, ErrorMessage = ErrorResponses.SubmissionNotFound($"{userName}_{name}") });
		}

		[HttpGet("{gameType}/submissions")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult<string> GetUserSubmissions(string gameType)
		{
			List<LoneAntSubmissionResponse> userSubmissions = _fileService.GetSubmissionsJson(gameType);
			if(userSubmissions.Count == 0)
			{
				return NoContent();
			}
			return Ok(userSubmissions);
		}

		[HttpGet("{gameType}/submission-names")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult<string> GetUserSubmissionNames(string gameType)
		{
			List<string> userSubmissionNames = _fileService.GetSubmissionNames(gameType);
			if (userSubmissionNames.Count == 0)
			{
				return NoContent();
			}
			return Ok(userSubmissionNames);
		}
	}
}
