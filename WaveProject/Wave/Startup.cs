using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Wave.Database;
using Wave.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Wave.Models;
using Wave.Interfaces;
using Wave.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Auth0.ManagementApi;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using RestSharp;
using Newtonsoft.Json;

namespace Wave
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
            services.AddControllers();

            services.AddAutoMapper(typeof(MapperConfig));

            services.AddSignalR();

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var authority = Configuration["Auth0ApiConfig:Authority"];
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = Configuration["Auth0ApiConfig:Identifier"];

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notification"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
                
                options.TokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier;
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddOptions();
            services.Configure<AzureBlobConfig>(Configuration.GetSection("AzureBlobConfig"));
            services.Configure<Auth0ApiConfig>(Configuration.GetSection("Auth0ApiConfig"));

            var connString = Configuration["AzureBlobConfig:ConnectionString"];

            services.AddSingleton<BlobServiceClient>(new BlobServiceClient(Configuration["AzureBlobConfig:ConnectionString"]));

            services.AddScoped<ManagementApiClient>(q => 
            {
                var domain = Configuration.GetSection("Auth0ApiConfig:Domain").Value;
                var clientId = Configuration.GetSection("Auth0ApiConfig:ClientId").Value;
                var clientSecret = Configuration.GetSection("Auth0ApiConfig:ClientSecret").Value;
                var client = new RestClient($"https://{domain}/oauth/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", $"{{\"client_id\":\"{clientId}\",\"client_secret\":\"{clientSecret}\",\"audience\":\"https://{domain}/api/v2/\",\"grant_type\":\"client_credentials\"}}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                Dictionary<string, string> resp = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

                return new ManagementApiClient(resp["access_token"], domain);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wave API", Version = "v1" });
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "Bearer" } } };
                c.AddSecurityRequirement(securityRequirement);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:admin", policy => policy.Requirements.Add(new HasScopeRequirement("read:admin", authority)));

                options.AddPolicy("write:admin", policy => policy.Requirements.Add(new HasScopeRequirement("write:admin", authority)));

                options.AddPolicy("modify:admin", policy => policy.Requirements.Add(new HasScopeRequirement("modify:admin", authority)));

                options.AddPolicy("remove:admin", policy => policy.Requirements.Add(new HasScopeRequirement("remove:admin", authority)));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(sw =>
                {
                    sw.SwaggerEndpoint("/swagger/v1/swagger.json", "Wave API V1");
                });
            }
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapHub<NotificationHub>("/notification");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
