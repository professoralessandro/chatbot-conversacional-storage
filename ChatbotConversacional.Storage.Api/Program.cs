#region REFERENCES
using ChatbotConversacionalStorage.Application.Helper.Settings;
using ChatbotConversacionalStorage.Application.Helper.Static.Generic;
using ChatbotConversacionalStorage.Application.Helper.Static.Serilog;
using ChatbotConversacionalStorage.Application.Helper.Static.Settings;
using ChatbotConversacionalStorage.Application.Helper.Static.Settings.Jtw;
using ChatbotConversacionalStorage.Domain.Context.Postgre;
using ChatbotConversacionalStorage.Infrastructure.Arquiteture.ServicesInjection;
using ChatbotConversacionalStorage.Infrastructure.Mappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Security.Cryptography;
using System.Text;
#endregion REFERENCES

#region TRY CATCH
try
{
    var builder = WebApplication.CreateBuilder(args);

    #region INTERNAL RUNTIMES
    var config = builder.Configuration;
    var env = builder.Environment;
    RumtimeSettings.ApiName = config.GetValue<string>("ApiConfiguration:ApiName");
    RumtimeSettings.ApiVersion = config.GetValue<string>("ApiConfiguration:Version");
    RumtimeSettings.ApiEnvironment = env.EnvironmentName;

    #region HTTP RUMTIME SETTINGS
    // Configure MailSettings
    builder.Services.Configure<HttpRumtimeSettings>(builder.Configuration.GetSection("HttpRumtimeSettings"));
    #endregion HTTP RUMTIME SETTINGS

    #region RUNTIMEVARIABLES
    #region CONNECTION STRING
    RumtimeSettings.ConnectionString = config.GetValue<string>("ConnectionString");
    RumtimeSettings.ConnectionStringPostgre = config.GetValue<string>("ConnectionStringPostgre");
    #endregion CONNECTION STRING

    #region API SECRET
    JwtRuntimeConfig.Secret = config.GetValue<string>("JwtConfig:Secret");
    JwtRuntimeConfig.RefreshTokenExpiryTimeInDay = config.GetValue<int>("JwtConfig:RefreshTokenExpiryTimeInDay");
    JwtRuntimeConfig.ExpiresInHour = config.GetValue<int>("JwtConfig:ExpiresInHour");
    JwtRuntimeConfig.Issuer = config.GetValue<string>("JwtConfig:Issuer");
    JwtRuntimeConfig.Audience = config.GetValue<string>("JwtConfig:Audience");
    #endregion API SECRET

    #region API POLICY
    RumtimeSettings.ApiPolicy = config.GetValue<string>("ApiPolicy");
    #endregion API POLICY

    #region TOKEN CONFIGURATION
    #endregion TOKEN CONFIGURATION
    #endregion
    #endregion RUNTIMEVARIABLES

    #region API CONTROLLERS ENDPOINT CONFIGURATION
    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    #endregion API CONTROLLERS ENDPOINT CONFIGURATION

    #region SWAGGER
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "Chatbot Storage",
                Version = "v1"
            }
        );
        options.SwaggerDoc("v2",
            new OpenApiInfo
            {
                Title = "Chatbot Storage",
                Version = "v2"
            }
        );
        options.AddSecurityDefinition("Bearer",
           new OpenApiSecurityScheme
           {
               Description = "Enter your JWT access token",
               Name = "Authorization",
               In = ParameterLocation.Header,
               Type = SecuritySchemeType.Http,
               Scheme = JwtBearerDefaults.AuthenticationScheme,
               BearerFormat = "JWT"
           }
       );

        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    });
    #endregion SWAGGER

    #region CORS CONFIGURATION
    builder.Services.AddCors(option =>
    {
        option.AddPolicy(RumtimeSettings.ApiPolicy, builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
    });
    #endregion

    #region SERILOG
    builder.Configuration.AddSerilogApi();
    // builder.Host.UseSerilog(Log.Logger);
    builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(LogEventLevel.Debug)
    .WriteTo.File("./SerilogStorage/log.txt",
        LogEventLevel.Warning,
        rollingInterval: RollingInterval.Day));
    #endregion

    #region ENTITY FRAMEWORK
    builder.Services.AddEntityFrameworkNpgsql().AddDbContext<APContextPostgre>(a => a.UseNpgsql(RumtimeSettings.ConnectionStringPostgre));
    #endregion ENTITY FRAMEWORK

    #region JWT CONFIGURATION

    var jwtKey = builder.Configuration["Jwt:Key"]
        ?? builder.Configuration["JwtConfig:Key"]
        ?? builder.Configuration["JwtConfig:Secret"];
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? builder.Configuration["JwtConfig:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? builder.Configuration["JwtConfig:Audience"];

    if (builder.Environment.IsDevelopment())
    {
        jwtIssuer ??= "ChatbotConversacional.Api";
        jwtAudience ??= "ChatbotConversacional.Frontend";
    }

    if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    {
        if (builder.Environment.IsEnvironment("Testing"))
            jwtKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        else
            throw new InvalidOperationException("Jwt:Key deve conter pelo menos 32 bytes.");
    }

    if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
        throw new InvalidOperationException("Jwt:Issuer e Jwt:Audience devem estar configurados.");

    builder.Configuration["JwtConfig:Key"] = jwtKey;
    builder.Configuration["JwtConfig:Issuer"] = jwtIssuer;
    builder.Configuration["JwtConfig:Audience"] = jwtAudience;
    builder.Configuration["Jwt:Key"] = jwtKey;
    builder.Configuration["Jwt:Issuer"] = jwtIssuer;
    builder.Configuration["Jwt:Audience"] = jwtAudience;

    var key = Encoding.UTF8.GetBytes(jwtKey);

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            //ValidateIssuerSigningKey = true,
            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtRuntimeConfig.Secret)),
            //ValidateAudience = false,
            //ValidateIssuer = false,
            //ValidateLifetime = true,
            //ClockSkew = TimeSpan.Zero

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },

            ValidIssuer = jwtIssuer,

            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
    #endregion JWT CONFIGURATION

    #region USE SERVICES
    // SERVICES
    builder.Services.AddServices();
    #endregion

    #region MAPPERS
    // MAPPER
    builder.Services.AddAutoMapper(typeof(MapperDtoToEntity), typeof(MapperEntityToDto));
    #endregion

    #region APP BUILD
    var app = builder.Build();
    #endregion

    #region CONFIGURE APP HTTPS REQUESTS METHOD
    #region DEVELOPMENT ENVRONMENT VALIDAION
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHttpsRedirection();
    }
    #endregion DEVELOPMENT ENVRONMENT VALIDAION

    #region API USE CONFIGURATION
    app.UseSerilogRequestLogging();

    #region CONFIGURARION CORS
    // app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    app.UseCors(RumtimeSettings.ApiPolicy);
    #endregion
    app.UseAuthentication();
    app.UseAuthorization();
    #endregion API USE CONFIGURATION

    app.MapControllers();

    app.Run();
    #endregion CONFIGURE APP HTTPS REQUESTS METHOD
}
catch (Exception ex)
{
    // SERILOG REQUEST LOG
    Log.Fatal(ex, UtilHelper.FormatLogInformationMessage(message: "Critical error => Host terminated unexpectedly", userId: Guid.Parse("d2a833de-5bb4-4931-a3c2-133c8994072a"), isHeaderOrFooter: true));
}
finally
{
    // SERILOG REQUEST LOG
    Log.Information(UtilHelper.FormatLogInformationMessage(message: "Critical error => Server Shutting down...", userId: Guid.Parse("d2a833de-5bb4-4931-a3c2-133c8994072a")));
    Log.CloseAndFlush();
}
#endregion TRY CATCH