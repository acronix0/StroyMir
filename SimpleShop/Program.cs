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
    private static void Main(string[] args)
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
        
        //builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder2 =>
        //{
        //    builder2.AllowAnyOrigin()
        //            .AllowAnyMethod()
        //            .AllowAnyHeader();
        //}));

        var app = builder.Build();
        configuration = app.Configuration;
        //app.UseCors("CorsPolicy");
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();
        if (app.Environment.IsDevelopment())
        {
            //UseSwagger
        }
        app.UseCors("AllowAllOrigins");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        using (var scope = app.Services.CreateScope())
        {
            if (configuration.GetValue<bool>("AutoMigrate"))
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<RepositoryContext>();

                dbContext.Database.Migrate();
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