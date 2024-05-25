using Microsoft.Extensions.Configuration;
using SimpleShop.Service.Extensions;
using SimpleShop.Service.Interfaces;
using SimpleShop.Service.Services;
internal class Program
{
    private static IConfiguration configuration;
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.ConfigureSqlContext(builder.Configuration);
        builder.Services.AddScoped<RepositoryManager>();
        builder.Services.AddScoped<OrderRepository>();
        

        //builder.Services.ConfigureRepositoryManager();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
       
        var app = builder.Build();
        configuration = app.Configuration;
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}