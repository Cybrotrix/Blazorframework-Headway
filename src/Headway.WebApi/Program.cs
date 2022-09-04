using FluentValidation.AspNetCore;
using Headway.Core.Constants;
using Headway.Core.Interface;
using Headway.RemediatR.Core.Interface;
using Headway.RemediatR.Repository;
using Headway.Repository;
using Headway.Repository.Data;
using Headway.SeedData;
using Headway.SeedData.RemediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSerilog((hostingContext, loggerConfiguration) =>
                  loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblies
        (new[] { Assembly.Load("Headway.RemediatR.Core") }))
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Headway.WebApi", Version = "v1" });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Configuration.GetConnectionString(DataMigrations.CONNECTION_STRING).Contains(DataMigrations.SQLITE_DATABASE))
    {
        options.EnableSensitiveDataLogging()
                .UseSqlite(builder.Configuration.GetConnectionString(DataMigrations.CONNECTION_STRING),
                            x => x.MigrationsAssembly(DataMigrations.SQLITE_MIGRATIONS));
    }
    else
    {
        options.EnableSensitiveDataLogging()
                .UseSqlServer(builder.Configuration.GetConnectionString(DataMigrations.CONNECTION_STRING),
                            x => x.MigrationsAssembly(DataMigrations.SQLSERVER_MIGRATIONS));
    }
});

builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IAuthorisationRepository, AuthorisationRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IOptionsRepository, OptionsRepository>();
builder.Services.AddScoped<IDemoModelRepository, DemoModelRepository>();
builder.Services.AddScoped<IRemediatRRepository, RemediatRRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("local",
        builder =>
            builder.WithOrigins("https://localhost:44300", "https://localhost:44310")
                   .AllowAnyHeader()
                   .AllowAnyMethod());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var identityProvider = builder.Configuration["IdentityProvider:DefaultProvider"];

    options.Authority = $"https://{builder.Configuration[$"{identityProvider}:Domain"]}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration[$"{identityProvider}:Domain"],
        ValidAudience = builder.Configuration[$"{identityProvider}:Audience"]
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Headway.WebApi v1"));
    app.UseSerilogRequestLogging();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("local");

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

var useSeedData = bool.Parse(builder.Configuration["SeedData:UseSeedData"]);
var useDefaultData = bool.Parse(builder.Configuration["SeedData:UseDefaultData"]);
var userRemediatRData = bool.Parse(builder.Configuration["SeedData:UserRemediatRData"]);

if(useSeedData)
{
    // Seed data for testing purposes only...
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();

        if(useDefaultData)
        {
            applicationDbContext.SetUser("Headway.SeedData");

            CoreData.Initialise(applicationDbContext);

            if (userRemediatRData)
            {
                RemediatRData.Initialise(applicationDbContext);
            }

            DeveloperData.Initialise(applicationDbContext);
        }
    }
}

app.Run();