using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Warehouse.Api.IntegrationTests.Authentication;
using Warehouse.Api.IntegrationTests.TestServices;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{   
    //Ai tool told me to change this because the product and supplier test classes were sharing the same in-memory Db name
    //The idea is that it creates a unique database for each application factory instance.
    private readonly string _databaseName = $"WarehouseIntegrationTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<WarehouseDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<IDbContextFactory<WarehouseDbContext>>();
            services.RemoveAll<WarehouseDbContext>();

            services.AddDbContextFactory<WarehouseDbContext>(
                options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });

            services.RemoveAll<ICacheService>();
            services.AddSingleton<ICacheService, TestCacheService>();
            services.RemoveAll<IFileStorageService>();
            services.AddSingleton<IFileStorageService, TestFileStorageService>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;

                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        ResetDatabase(host.Services);
        return host;
    }

    public void ResetDatabase()
    {
        ResetDatabase(Services);
    }

    private static void ResetDatabase(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedDatabase(context);
    }

    private static void SeedDatabase(
        WarehouseDbContext context)
    {
        var supplier = new Supplier("sup1", "lebanon", "sup1@mail.com", "+12-345-111");

        var products = new[]
        {
            new Product("keyboard2", "KEY-002", "budget keyboard", 20, 25, supplier, DateTime.UtcNow.AddYears(1)),

            new Product("mouse1", "MOU-001", "budget mouse", 10, 5, supplier, DateTime.UtcNow.AddYears(1))
        };

        context.Suppliers.Add(supplier);
        context.Products.AddRange(products);
        context.SaveChanges();
    }
}