﻿using CodeChallengeInc.SubmissionApi.Interfaces;
using CodeChallengeInc.SubmissionApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeChallengeInc.SubmissionApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors(Options =>
			{
				Options.AddPolicy("AllowAny", builder =>
				{
					builder.AllowAnyOrigin();
					builder.AllowAnyHeader();
					builder.AllowAnyMethod();
				});
			});
			services.AddScoped<IFileService, FileService>();
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}
			app.UseCors("AllowAny");
			using(IServiceScope scope = app.ApplicationServices.CreateScope())
			{
				scope.ServiceProvider.GetService<IFileService>().PurgeDefaultAnts("LoneAnt");
				scope.ServiceProvider.GetService<IFileService>().PurgeDefaultAnts("SpawningAnts");
			}
			app.UseMvc();
		}
	}
}
