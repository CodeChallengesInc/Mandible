using CodeChallengeInc.Mandible.Controllers;
using CodeChallengeInc.Mandible.Interfaces;
using CodeChallengeInc.Mandible.Services;
using System.IO.Abstractions;

namespace CodeChallengeInc.Mandible
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
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<LoneAntController>), typeof(Logger<LoneAntController>));
            services.AddLogging();
			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
			app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
			{
				scope.ServiceProvider.GetService<IFileService>()!.PurgeDefaultAnts();
			}
		}
	}
}
