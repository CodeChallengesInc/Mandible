using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeChallengeInc.SubmissionApi.Constants;
using CodeChallengeInc.SubmissionApi.Models;
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
			PrepareEnvironment("LoneAnt");
			PrepareEnvironment("SpawningAnts");
			PrepareEnvironment("FormicAnts");
			CreateWebHostBuilder(args).Build().Run();
		}

		public static void PrepareEnvironment(string gameType)
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			if (!Directory.Exists(Path.Combine(currentDirectory, FileInformation.SubmissionFolder)))
			{
				Directory.CreateDirectory(Path.Combine(currentDirectory, FileInformation.SubmissionFolder));
			}
			if(!Directory.Exists(Path.Combine(currentDirectory, FileInformation.SubmissionFolder, gameType)))
			{
				Directory.CreateDirectory(Path.Combine(currentDirectory, FileInformation.SubmissionFolder, gameType));
			}
			if(!Directory.Exists(Path.Combine(currentDirectory, FileInformation.BackupSubmissionFolder, gameType)))
			{
				Directory.CreateDirectory(Path.Combine(currentDirectory, FileInformation.BackupSubmissionFolder, gameType));
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
