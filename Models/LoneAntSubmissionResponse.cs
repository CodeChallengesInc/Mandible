using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CodeChallengeInc.SubmissionApi.Models
{
	public class LoneAntSubmissionResponse : LoneAntSubmission
	{
		public string Name { get; set; }
	}
}
