using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleShop.Core.Model;
using SimpleShop.Repo.Data;
using SimpleShop.Service.Extensions;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;
using SimpleShop.WebApi.Controllers;
using System.Security.Cryptography;
internal class Program
{
    private static IConfiguration configuration;
     static void Main(string[] args)
    {
      
       
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.ConfigureSqlContext(builder.Configuration);
        builder.Services.AddScoped<BasketManager>();
        builder.Services.AddScoped<RepositoryManager>();
        builder.Services.AddSingleton<MailManager>();
        builder.Services.ConfigureLoggerService();
        builder.Services.ConfigureMapping();
        
        builder.Services.ConfigureRepositoryManager();
        builder.Services.AddScoped<ImportManager>();
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 104857600; // 100 MB
        });
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication();
        builder.Services.ConfigureIdentity();
        builder.Services.ConfigureSwagger();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });
        builder.Services.ConfigureJWT(builder.Configuration);

        var app = builder.Build();
        configuration = app.Configuration;
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.Urls.Add("http://*:7233");
        }

        app.UseCors("AllowAllOrigins");
        //app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        using (var scope = app.Services.CreateScope())
        {
            if (configuration.GetValue<bool>("AutoMigrate"))
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<RepositoryContext>();
                try
                {
                    dbContext.Database.Migrate();
                }
                catch 
                {

                }
                
            }

            var repositoryManager = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();
            CreateRolesIfNotExist(repositoryManager);


        }
        
        app.Run();
    }
    private static void CreateRolesIfNotExist(IRepositoryManager repositoryManager)
    {
        foreach (var role in UserRoles.AllRoles)
        {
            repositoryManager.UserAuthentication.AddRoleIfNotExistAsync(role).Wait();
        }
    }
}