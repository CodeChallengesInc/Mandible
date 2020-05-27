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
	[Route("LoneAnt")]
	[ApiController]
	public class LoneAntController : ControllerBase//, IGameController
	{
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
			string submissionLocation = Path.Combine(GetSubmissionsPath(), username + FileInformation.LoneAntFileExtension);
			if (System.IO.File.Exists(submissionLocation))
			{
				return JsonConvert.SerializeObject(new LoneAntSubmission(username, System.IO.File.ReadAllText(submissionLocation)));
			}
			return NotFound(ErrorResponses.SubmissionNotFound(username));
		}

		[HttpPut("{username}")]
		public ActionResult SubmitUserEntry(string username, [FromBody] string submission)
		{
			string submissionLocation = Path.Combine(GetSubmissionsPath(), username + FileInformation.LoneAntFileExtension);
			if (System.IO.File.Exists(submissionLocation))
			{
				BackupFile(submissionLocation);
			}
			if(submission != string.Empty)
			{
				CreateOrOverwriteSubmission(submissionLocation, submission);
				return NoContent();
			}
			return BadRequest(ErrorResponses.SubmissionPutBodyEmpty);
		}

		[HttpDelete("{username}")]
		public ActionResult DeleteSubmission(string username)
		{
			string submissionLocation = Path.Combine(GetSubmissionsPath(), username + FileInformation.LoneAntFileExtension);
			if (System.IO.File.Exists(submissionLocation))
			{
				BackupFile(submissionLocation);
				System.IO.File.Delete(submissionLocation);
				return NoContent();
			}
			return NotFound(ErrorResponses.SubmissionNotFound(username));
		}

		public string GetSubmissionsPath()
		{
			return Path.Combine(FileInformation.LoneAntFolder, FileInformation.SubmissionFolder);
		}

		public string GetBackupsPath()
		{
			return Path.Combine(FileInformation.LoneAntFolder, FileInformation.BackupSubmissionFolder);
		}
		public void BackupFile(string path)
		{
			string content = System.IO.File.ReadAllText(path);
			FileInfo fi = new FileInfo(path);

			string filename = $"{fi.Name.Replace(FileInformation.LoneAntFileExtension, string.Empty)} - {fi.LastWriteTime.ToString(FileInformation.DateTimeFormat)}{fi.Extension}";
			string backupLocation = Path.Combine(GetBackupsPath(), filename);

			System.IO.File.WriteAllText(backupLocation, content);
		}
		public void CreateOrOverwriteSubmission(string path, string submissionText)
		{
			using(FileStream fs = System.IO.File.Create(path))
			{
				byte[] content = new UTF8Encoding(true).GetBytes(submissionText);

				fs.Write(content, 0, content.Length);
			}
		}
	}
}
