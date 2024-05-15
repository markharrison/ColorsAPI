using System;
using ColorsAPI.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;

namespace ColorsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ColorsService cs = new ColorsService(builder.Configuration);
            builder.Services.AddSingleton(cs);

            builder.Services.AddControllers();
            builder.Services.AddCors();
            builder.Services.AddSwaggerGen(c =>
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCors(builder =>
                      builder.WithOrigins("http://localhost")
                              .AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod());

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(1)
                    };
                }
            });

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

            app.MapGet("/appconfiginfo", async context =>
            {
                await context.Response.WriteAsync(cs.GetAppConfigInfo(context));
            });

            app.MapControllers();

            app.Run();
        }
    }
}
