//
//  Swagger: http://localhost:5110/swagger/index.html
//  Authentication: http://localhost:5110/Authentication
//  Service: http://localhost:5110/Service
//
//  Pages
//      DataHub: http://localhost:5110/DataHub  --> Only if connection is on


using Arpal.SiApi.WebApplication;
using Arpal.SiApi.WebApplication.Database;
using Arpal.SiApi.WebApplication.GraphQL.Schema;
using Arpal.SiApi.WebApplication.JWT;
using Arpal.SiApi.WebApplication.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Mime;
using System.Text;


string SiteAllowSpecificOrigins = "_siteAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

Boolean swaggerEnabled = builder.Configuration.GetValue<bool>(Consts.Swagger_Enabled);
string? swaggerExternalAddress = builder.Configuration.GetValue<string>(Consts.Swagger_ExternalAddress);


builder.Services.AddRazorPages();

// ****************************************************************************************************
//  Authentication / Authorization
// ****************************************************************************************************
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        byte[] signingKeyBytes = Encoding.UTF8.GetBytes(TokenManager.SecurityKey);

        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration[Consts.Jwt_IssuerName],
            ValidAudience = builder.Configuration[Consts.Jwt_AudienceName],
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
        };
    });

// ****************************************************************************************************
//  Controller
// ****************************************************************************************************
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "SiAPI", Version = "v1" });

    if (!string.IsNullOrWhiteSpace(swaggerExternalAddress))
        option.AddServer(new OpenApiServer { Url = swaggerExternalAddress, Description = "Arpal Server" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        // This create an error in Yaml
        // BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// ****************************************************************************************************
//  GraphQL
// ****************************************************************************************************
builder.Services.AddPooledDbContextFactory<SiApiContext>(o =>
{
    o.UseOracle("");
});

builder.Services.AddGraphQLServer().AddQueryType<Query>();

// ****************************************************************************************************
//  Logging
// ****************************************************************************************************
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddLog4Net("log4net.config");
});

// ****************************************************************************************************
//  CORS
// ****************************************************************************************************
builder.Services.AddCors(options => options.AddPolicy(SiteAllowSpecificOrigins, builder =>
{
    builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials();
}));

// *******************************************************************************************
// Request Model Validation, this is used not to provide too many details in response
//          Reference in this class: ValidationResultModel
// *******************************************************************************************
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var result = new ValidationFailedResult(context);
        result.ContentTypes.Add(MediaTypeNames.Application.Json);
        return result;
    };
});

// ****************************************************************************************************
// ****************************************************************************************************
//  USE
// ****************************************************************************************************
// ****************************************************************************************************
var app = builder.Build();

// Add the exception handling middleware to log exceptions globally
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            app.Logger.LogError($"Message: {exceptionHandlerPathFeature.Error.Message}, StackTrace: {exceptionHandlerPathFeature.Error.StackTrace}");
        }

        context.Response.ContentType = "text/html; charset=utf-8";

        await context.Response.WriteAsync($"Il servizio è al momento non disponibile. Si prega di riprovare più tardi.");
    });
});

// Configure the HTTP request pipeline.
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SiApi v1");
        // Configure Swagger UI to serve YAML instead of JSON
        c.SwaggerEndpoint("/swagger/v1/swagger.yaml", "SiApi v1 (YAML)");
    });
}

app.UseRouting();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(SiteAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

// ****************************************************************************************************
//  GraphQL
// ****************************************************************************************************
// Se abilito useRouting non funziona + authenticazione
//app.UseRouting();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGraphQL();
//});

app.MapRazorPages();

app.MapControllers();

app.Run();