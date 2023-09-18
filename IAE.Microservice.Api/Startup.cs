using AutoMapper;
using FluentValidation;
using IAE.Microservice.Application.Common.CacheProviders;
using IAE.Microservice.Application.Features.Accounts.Notifications;
using IAE.Microservice.Application.Infrastructure;
using IAE.Microservice.Application.Infrastructure.AutoMapper;
using IAE.Microservice.Application.Interfaces;
using IAE.Microservice.Application.Notifications.Models;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IAE.Microservice.Api.Configurations;
using IAE.Microservice.Api.Filters;
using IAE.Microservice.Api.Infrastructure;
using IAE.Microservice.Api.Security;
using IAE.Microservice.Application.Common.HClient;
using IAE.Microservice.Application.Common.Jobs;
using IAE.Microservice.Application.Features.Accounts;
using IAE.Microservice.Application.Interfaces.Social;
using IAE.Microservice.Domain.Entities.Users;
using IAE.Microservice.Infrastructure;
using IAE.Microservice.Infrastructure.Social.Client;
using IAE.Microservice.Persistence;
using Microsoft.Extensions.Hosting;
using NSwag.Generation.AspNetCore;
using SocialMicroservice = IAE.Microservice.Infrastructure.Social;

namespace IAE.Microservice.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add environment config
            EnvironmentConfig.Init(Configuration.GetValue("Environment:HideExceptions", false),
                Configuration.GetValue<int>("Environment:TtlInSec"),
                Configuration.GetValue("Environment:IsDmpOff", false),
                Configuration.GetValue("Environment:IsDmpExApi2", true));

            // Add AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

                // Add FluentValidation config
            ValidatorOptions.LanguageManager.Culture = new CultureInfo("en-US");

            // Add RequestLogging
            services.AddTransient<RequestLogging.Middleware>();
            services.AddTransient<RequestLogging.HttpClientMessageHandler>();

            // Add RequestValidator
            services.AddTransient<RequestValidator.Middleware>();

            // Add Social microservice
            services.AddScoped<ISocialUserService, SocialMicroservice.Endpoints.Users.UserService>();
            services.AddScoped<ISocialChatService, SocialMicroservice.Endpoints.Chats.ChatService>();
            
            // Add Social microservice HttpClient
            var socialConfig = Configuration.GetSection("SocialMicroservice").Get<HClientConfig>();
            services.AddHttpClient<ISocialClient, SocialClient>()
                .AddHttpMessageHandler<RequestLogging.HttpClientMessageHandler>()
                .Configure(socialConfig, headers => { headers.Add("X-Auth-Token", socialConfig.AdminToken); });

            
            // Add framework services
            services.AddHttpContextAccessor();
            services.Configure<SmtpManagement>(Configuration.GetSection("SmtpManagement"));
            services.Configure<NotificationManagement>(Configuration.GetSection("NotificationManagement"));
            services.AddSingleton<INotificationSender, EmailNotificationSender>();
            services.AddSingleton<IUserAgentParser, RegexUserAgentParser>();
            services.AddSingleton<IGeoByIPSearcher, MaxmindGeoByIPSearcher>();
            services.AddTransient<MessageProvider>();
            
            // Scheduler && Jobs
            services.AddScheduler();
            
            // Add MediatR
            services.AddMediatR(typeof(ChangePassword.Command).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            // Add DbContext using Postgres Provider
            services.Configure<AdminUser>(Configuration.GetSection("AdminUser"));
            services.AddDbContext<MicroserviceDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("TradingDeskDatabase")));
            services.AddTransient<IMicriserviceDbContext, MicroserviceDbContext>();

            services.Configure<DataProtectionTokenProviderOptions>(x => x.TokenLifespan = TimeSpan.FromDays(30));
            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = true;
                    options.User.RequireUniqueEmail = true;

                    //For email confirmations and reset passwords using email token provider that generate 6 digits short lived code.
                    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                })
                .AddEntityFrameworkStores<MicroserviceDbContext>()
                .AddDefaultTokenProviders();

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Role.Administrator)
                .Build();
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new AuthorizeFilter(policy));
                    options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.Admin, p => p.RequireRole(Role.Administrator));
            });

            ValidatorOptions.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;

            //Add authentication            
            services.Configure<TokenManagement>(Configuration.GetSection("TokenManagement"));
            var token = Configuration.GetSection("TokenManagement").Get<TokenManagement>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = token.SecurityKey,
                    ValidIssuer = token.Issuer,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = token.Audience,
                    ValidateLifetime = true,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });
            services.AddTransient<ITokenProvider, JwtTokenProvider>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddTransient<CurrentUserMiddleware>();

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            var swaggerInfo = Configuration.GetSection("Swagger:Info");
            services
                .AddSwaggerDocument(configure => { BaseConfigure(configure, swaggerInfo); });

            // Memory Cache
            services.AddMemoryCache();
            services.AddScoped<ICacheProvider, MemoryCacheProvider>();
        }

        private void BaseConfigure(AspNetCoreOpenApiDocumentGeneratorSettings configure,
            IConfigurationSection info)
        {
            configure.PostProcess = document =>
            {
                document.Info.Title = info["Title"];
                document.Info.Description = info["Description"];
            };
            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the text box: Bearer {your JWT token}."
            });
            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var forwardedOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                ForwardLimit = 10
            };
            forwardedOptions.KnownProxies.Clear();
            forwardedOptions.KnownNetworks.Clear();
            app.UseForwardedHeaders(forwardedOptions);

            app.UseCors(policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });

            app.UseMiddleware<RequestLogging.Middleware>();
            app.UseMiddleware<RequestValidator.Middleware>();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi(config =>
            {
                config.PostProcess = (document, request) =>
                {
                    document.Schemes = new[] { OpenApiSchema.Https, OpenApiSchema.Http };
                    var swaggerHost = Configuration["Swagger:Host"];
                    if (!string.IsNullOrEmpty(swaggerHost))
                        document.Host = swaggerHost;
                };
            });
            app.UseSwaggerUi3();

            app.UseApiVersioning();

            app.UseScheduler();
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CurrentUserMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}