using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeChallengeInc.Mandible.Constants;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeChallengeInc.Mandible
{
	public class Program
	{
		public static void Main(string[] args)
		{
			PrepareEnvironment();
			CreateWebHostBuilder(args).Build().Run();
		}

        public static void PrepareEnvironment()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string loneAntFolder = Path.Combine(currentDirectory, FileInformation.LoneAntFolder);
            string submissionFolder = Path.Combine(loneAntFolder, FileInformation.SubmissionFolder);
            string backupSubmissionFolder = Path.Combine(loneAntFolder, FileInformation.BackupSubmissionFolder);

            EnsureDirectoryExists(loneAntFolder);
            EnsureDirectoryExists(submissionFolder);
            EnsureDirectoryExists(backupSubmissionFolder);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
