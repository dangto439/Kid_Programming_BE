using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Services.Services;
using KidPrograming.Core.Base;
using KidPrograming.Repositories.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using KidPrograming.Core.Utils;
namespace KidPrograming.DI
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.JwtSettingsConfig(configuration);
            services.AddAuthenJwt(configuration);
            services.AddRedis(configuration);
            services.ConfigSwagger();
            services.ConfigCors();
            services.InitSeedData();
            services.AddFirebase();
        }
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<KidProgramingDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
            });
        }
        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            RedisConfiguration redisSetting = new RedisConfiguration();
            configuration.GetSection("RedisConfiguration").Bind(redisSetting);

            services.AddSingleton(redisSetting);

            if (!redisSetting.Enabled)
                return;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisSetting.ConnectionString));
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisSetting.ConnectionString;
                });
            services.AddSingleton<ICacheService,CacheService>();

        }
        public static void AddFirebase(this IServiceCollection services)
        {
            string credentialPath = "firebase-adminsdk.json";

            if (!File.Exists(credentialPath))
            {
                throw new FileNotFoundException("Firebase credential file not found!", credentialPath);
            }

            var credential = GoogleCredential.FromFile(credentialPath)
                .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

            FirebaseApp app;
            if (FirebaseApp.DefaultInstance == null)
            {
                app = FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }
            else
            {
                app = FirebaseApp.DefaultInstance;
            }

            services.AddSingleton(credential);

            services.AddSingleton(app);
            services.AddSingleton(provider => FirebaseAuth.GetAuth(provider.GetRequiredService<FirebaseApp>()));
            services.AddSingleton(provider => FirebaseMessaging.GetMessaging(provider.GetRequiredService<FirebaseApp>()));

            services.AddSingleton<FirebaseAuthHelper>();
        }


        public static void AddAuthenJwt(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSettings jwtSettings = services.BuildServiceProvider().GetRequiredService<JwtSettings>();
            services.AddAuthentication(e =>
            {
                e.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                e.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(e =>
            {
                e.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!))

                };
                e.SaveToken = true;
                e.RequireHttpsMetadata = true;
            });
        }
        public static void JwtSettingsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(option =>
            {
                JwtSettings jwtSettings = new JwtSettings
                {
                    SecretKey = configuration.GetValue<string>("JwtSettings:SecretKey"),
                    Issuer = configuration.GetValue<string>("JwtSettings:Issuer"),
                    Audience = configuration.GetValue<string>("JwtSettings:Audience"),
                    AccessTokenExpirationMinutes = configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes")
                };
                jwtSettings.IsValid();
                return jwtSettings;
            });

        }
        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins("*")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
        }
        public static void ConfigSwagger(this IServiceCollection services)
        {
            // config swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "API"

                });
               
                // Thêm JWT Bearer Token vào Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header sử dụng scheme Bearer.",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Name = "Authorization",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }
        public static void InitSeedData(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KidProgramingDbContext>();
            var initialiser = new SeedData(context);
            initialiser.Initialise().Wait();
        }
    }
}
