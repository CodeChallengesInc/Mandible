using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.Mandible.Constants
{
	public class ErrorResponses
	{
		public static string SubmissionNotFound(string username) => $"{username}'s submission not found.";
		public static string SubmissionPutBodyEmpty => "Your request didn't contain a submission.";
	}
}
