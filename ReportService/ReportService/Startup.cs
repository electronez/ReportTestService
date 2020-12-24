namespace ReportService
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Npgsql;
    using Serilog;
    using Services;

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
            services.AddMvc();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped(o =>
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_ConnectionString");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Отсутствует строка подключения к БД");
                }
                return new NpgsqlConnection(connectionString);
            });
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
                app.UseExceptionHandler(x =>
                {
                    x.Run(async contect =>
                    {
                        var exceptionHandlerPathFeature = contect.Features.Get<IExceptionHandlerPathFeature>();
                        if (exceptionHandlerPathFeature?.Error != null)
                        {
                            Log.Logger.Error(exceptionHandlerPathFeature.Error, "Error on query {@path}", contect.Request.Path);
                            contect.Response.StatusCode = 200;
                            contect.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new
                            {
                                IsSuccess = false,
                                error = "Произошла ошибка, обратитесь к разработчикам",
                            });
                            await contect.Response.WriteAsync(result);
                        }
                    });
                });
            }

            app.UseMvc();
        }
    }
}
