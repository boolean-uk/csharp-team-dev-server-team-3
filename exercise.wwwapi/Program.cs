using exercise.wwwapi.Configuration;
using exercise.wwwapi.Data;
using exercise.wwwapi.Endpoints;
using exercise.wwwapi.EndPoints;
using exercise.wwwapi.Helpers;
using exercise.wwwapi.Models;
using exercise.wwwapi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var config = new ConfigurationSettings();

// Add services to the container.
builder.Services.AddScoped<IConfigurationSettings, ConfigurationSettings>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<ILogger, Logger<string>>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<DataContext>(options => {
    
    //options.UseNpgsql(builder.Configuration.GetConnectionString("LocalDatabase"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalDatabase"));
    options.LogTo(message => Debug.WriteLine(message));

});


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue("AppSettings:Token"))),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false

    };
});
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "C# Team Development Server",
        Description = "A .Net Minimal API with JWT Auth",
    });
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Add an Authorization header with a JWT token using the Bearer scheme see the app.http file for an example.)",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

});
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
    });

    app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v3.json", "Demo API");
    });
    app.MapScalarApiReference();
}

app.UseCors(x => x
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true) // allow any origin
                  .AllowCredentials()); // allow credentials

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.ConfigureAuthApi();

app.ConfigureSecureApi();

app.ConfigureLogEndpoints();

app.ConfigureCohortEndpoints();

app.ConfigurePostEndpoints();

app.ConfigureValidationEndpoint();

app.Run();

public partial class Program { } // needed for testing - please ignore