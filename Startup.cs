using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace proj
{
	public class Startup
	{
		public IConfiguration Configuration {get; private set;}

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
			.SetBasePath(env.ContentRootPath)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange:true)
			.AddEnvironmentVariables();
			Configuration = builder.Build();
		}
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddSession();
			services.Configure<MySqlOptions>(Configuration.GetSection("DBInfo"));
			services.AddScoped<DbConnector>();
			services.AddDbContext<projContext>(options => options.UseMySQL(Configuration["DBInfo:ConnectionString"]));
		}
		public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();
			app.UseDeveloperExceptionPage();
			app.UseStaticFiles();
			app.UseSession();
			app.UseMvc();
		}
	}
}
