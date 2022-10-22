using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColorsAPI.Services;
using Microsoft.AspNetCore.Http;

namespace ColorsAPI
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            config = configuration;
        }

        public IConfiguration config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new ColorsService(config));
            services.AddControllers();
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Mark Harrison Colors API",
                    Version = "v1",
                    Description = "Colors API",
                    TermsOfService = new Uri("https://github.com/markharrison/ColorsAPI/blob/master/LICENSE"),
                    Contact = new OpenApiContact
                    {
                        Name = "Mark Harrison",
                        Email = "mark.colorsapi@harrison.ws",
                        Url = new Uri("https://github.com/markharrison/ColorsAPI"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT License",
                        Url = new Uri("https://github.com/markharrison/ColorsAPI/blob/master/LICENSE"),
                    }
                }
                );

                c.EnableAnnotations();

            });
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ColorsService cs)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
              builder.WithOrigins("http://localhost")
                      .AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
                {
                    var basePath = "/";
                    var host = httpRequest.Host.Value;
                    var scheme = (httpRequest.IsHttps || httpRequest.Headers["x-forwarded-proto"].ToString() == "https") ? "https" : "http";

                    if (httpRequest.Headers["x-forwarded-host"].ToString() != "")
                    {
                        host = httpRequest.Headers["x-forwarded-host"].ToString() + ":" + httpRequest.Headers["x-forwarded-port"].ToString();
                    }

                    swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{scheme}://{host}{basePath}" } };

                });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mark Harrison Colors API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/appconfiginfo", async context => await context.Response.WriteAsync(cs.GetAppConfigInfo(context)));
                endpoints.MapControllers();
            });
        }
    }
}
