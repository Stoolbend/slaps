using AutoMapper;
using BCI.SLAPS.Server.Hubs;
using BCI.SLAPS.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace BCI.SLAPS.Server
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
            // Configure AutoMapper
            services.AddAutoMapper(typeof(DefaultMapperProfile));

            // Configure SignalR + Protocols
            services.AddSignalR();

            // Configure memory cache
            services.AddMemoryCache();

            // Configure EF Core
            services.AddDbContext<DataContext>();

            // Configure local services
            services.AddScoped<IContentService, ContentService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IUserService, UserService>();

            // Create ServiceProvider for service consumption
            var sp = services.BuildServiceProvider();

            // Configure JWT authentication
            // - Get required settings for token validation
            var _settings = sp.GetService<ISettingsService>();

            // - Configure token validation
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userSvc = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var user = await userSvc.GetUserAsync(Guid.Parse(context.Principal.Identity.Name));
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/rtc", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_settings.GetJwtSecret()),
                    ValidateIssuer = true,
                    ValidIssuer = _settings.GetJwtIssuer(),
                    ValidateAudience = true,
                    ValidAudience = _settings.GetJwtAudience()
                };
            });

            // Configure authorization
            services.AddAuthorizationCore(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
            });

            // Finally, add controllers
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            UpdateDatabase(app);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/rtc/chat");
                endpoints.MapHub<ContentHub>("/rtc/content");
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
