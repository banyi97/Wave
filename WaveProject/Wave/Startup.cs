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

            services.Configure<AzureBlobConfig>(Configuration.GetSection("AzureBlobConfig"));

            var connString = Configuration["AzureBlobConfig:ConnectionString"];

            services.AddSingleton<BlobServiceClient>(new BlobServiceClient(Configuration["AzureBlobConfig:ConnectionString"]));

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
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(sw =>
            {
                sw.SwaggerEndpoint("/swagger/v1/swagger.json", "Wave API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notification");
            });
        }
    }
}
