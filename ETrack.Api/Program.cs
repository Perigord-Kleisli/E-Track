using DotNet.RateLimiter;
using ETrack.Api.Data;
using ETrack.Api.Repositories;
using ETrack.Api.Repositories.Contracts;
using ETrack.Api.Services;
using ETrack.Api.Services.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContextPool<ETrackDBContext>(options => {
            options.UseSqlite(builder.Configuration.GetConnectionString("ETrackApiConnection"));
            options.LogTo(Console.WriteLine);
        });
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IStudentRepository, StudentRepository>();
        builder.Services.AddScoped<IEmailService,EmailService>();
        builder.Services.AddRateLimitService(builder.Configuration);
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        if (builder.Configuration["AppSettings:Token"] is null) 
            throw new Exception("User secret `AppSettings:Token` not found");
        if (builder.Configuration["Smtp:Host"] is null) 
            throw new Exception("User secret `Smtp:Host` not found");
        if (builder.Configuration["Smtp:Pass"] is null) 
            throw new Exception("User secret `Smtp:Pass` not found");
        if (builder.Configuration["Smtp:User"] is null) 
            throw new Exception("User secret `Smtp:User` not found");

        builder
            .Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration["AppSettings:Token"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseStaticFiles();

        app.MapControllers();

        app.Run("http://localhost:5292");
    }
}