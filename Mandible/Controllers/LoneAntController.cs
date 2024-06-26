﻿using Microsoft.AspNetCore.Mvc;
using CodeChallengeInc.Mandible.Interfaces;
using CodeChallengeInc.Mandible.Constants;
using CodeChallengeInc.Mandible.Models;

namespace CodeChallengeInc.Mandible.Controllers
{
	[Route("lone-ant")]
	[ApiController]
	public class LoneAntController : ControllerBase, IGameController
	{
		internal string Passcode { get; set; }
		IFileService _fileService;
		ILogger _logger;
		public LoneAntController(IFileService fileService, ILogger<LoneAntController> logger)
		{
			_fileService = fileService;
			_logger = logger;
			Passcode = (Environment.GetEnvironmentVariable(EnvironmentConstants.PasscodeVariable) != null) ? Environment.GetEnvironmentVariable(EnvironmentConstants.PasscodeVariable)! : EnvironmentConstants.DefaultPasscode!;
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
			if (_fileService.UserSubmissionExists(userName, antName))
			{
				return Ok(_fileService.GetUserSubmission(userName, antName));
			}
			return NotFound(new ErrorResponse { ErrorCode = 404, ErrorMessage = ErrorResponses.SubmissionNotFound($"{userName}_{antName}") });
		}

		[HttpPut("{userName}/{antName}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[Produces("application/json")]
		public ActionResult SubmitUserEntry(string userName, string antName, [FromBody] string submission)
		{
            if (string.IsNullOrEmpty(submission))
            {
                return BadRequest(new ErrorResponse { ErrorCode = 400, ErrorMessage = "Submission cannot be empty." });
            }

            try
			{
                _fileService.CreateOrOverwriteUserSubmission(userName, antName, submission);
                return NoContent();
            }
			catch (IOException ioEx)
			{
                _logger.LogError(ioEx, "Failed to create or overwrite user submission. IO Error.");
                return StatusCode(500, new ErrorResponse { ErrorCode = 500, ErrorMessage = "An error occurred while processing your request." });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument provided.");
                return BadRequest(new ErrorResponse { ErrorCode = 400, ErrorMessage = "Invalid argument provided." });
            }
            catch (Exception e)
			{
                _logger.LogError(e, "Failed to create or overwrite user submission. Unknown error.");
                return StatusCode(500, new ErrorResponse { ErrorCode = 500, ErrorMessage = "An error occurred while processing your request." });
            }
		}

		[HttpDelete("{userName}/{antName}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public ActionResult DeleteSubmission(string userName, string antName)
		{
			string fileName = $"{userName}_{antName}";
			if (_fileService.UserSubmissionExists(userName, antName))
			{
				_fileService.DeleteUserSubmission(userName, antName);
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
			List<LoneAntSubmissionResponse> userSubmissions = _fileService.GetSubmissionsReponse();
			if(userSubmissions.Count == 0)
			{
                return NotFound(new ErrorResponse { ErrorCode = 404, ErrorMessage = "No submissions found." });
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
