using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeChallengeInc.SubmissionApi.Constants;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeChallengeInc.SubmissionApi
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
			if (!Directory.Exists(Path.Combine(currentDirectory, FileInformation.LoneAntFolder)))
			{
				Directory.CreateDirectory(Path.Combine(currentDirectory, FileInformation.LoneAntFolder));
			}
			if(!Directory.Exists(Path.Combine(currentDirectory, FileInformation.LoneAntFolder, FileInformation.SubmissionFolder)))
			{
				Directory.CreateDirectory(Path.Combine(currentDirectory, FileInformation.LoneAntFolder, FileInformation.SubmissionFolder));
			}
			if(!Directory.Exists(Path.Combine(currentDirectory, FileInformation.LoneAntFolder, FileInformation.BackupSubmissionFolder)))
			{
				Directory.CreateDirectory(Path.Combine(currentDirectory, FileInformation.LoneAntFolder, FileInformation.BackupSubmissionFolder));
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
