using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CodeChallengeInc.Mandible.Models
{
	public class LoneAntSubmissionResponse : LoneAntSubmission
	{
		public string AntName { get; set; }
	}
}
