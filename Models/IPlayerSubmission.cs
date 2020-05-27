using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Models
{
	interface IPlayerSubmission
	{
		string Username { get; set; }
		string Submission { get; set; }
	}
}
