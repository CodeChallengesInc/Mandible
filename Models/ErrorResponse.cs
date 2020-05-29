using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Models
{
	public class ErrorResponse
	{
		public int ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
	}
}
