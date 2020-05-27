using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Models
{
	public class LoneAntSubmission : IPlayerSubmission
	{
		public string Username { get; set; }
		public string Submission { get; set; }

		public LoneAntSubmission(string username, string submission)
		{
			Username = username;
			Submission = submission;
		}
	}
}
