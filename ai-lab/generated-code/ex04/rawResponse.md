tests/Warehouse.Api.IntegrationTests/Fakes/TestFileStorageService.cs
using Warehouse.Application.Interfaces;

namespace Warehouse.Api.IntegrationTests.Fakes;

public class TestFileStorageService : IFileStorageService
{
private readonly Dictionary<string, StoredFile> _files = new();

    public IReadOnlyDictionary<string, StoredFile> Files => _files;

    public async Task UploadAsync(
        Stream fileStream,
        long fileSize,
        string objectKey,
        string contentType,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();

        await fileStream.CopyToAsync(memoryStream, cancellationToken);

        _files[objectKey] = new StoredFile(
            memoryStream.ToArray(),
            fileSize,
            contentType);
    }

    public Task<byte[]> DownloadAsync(
        string objectKey,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_files[objectKey].Content);
    }

    public Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken)
    {
        _files.Remove(objectKey);
        return Task.CompletedTask;
    }

    public Task<string> GetPresignedUrlAsync(
        string objectKey,
        int expirySeconds = 1000)
    {
        return Task.FromResult(
            $"https://test-storage.local/{objectKey}");
    }

    public void Reset()
    {
        _files.Clear();
    }

    public record StoredFile(
        byte[] Content,
        long FileSize,
        string ContentType);
}
tests/Warehouse.Api.IntegrationTests/CustomWebApplicationFactory.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Warehouse.Api.IntegrationTests.Authentication;
using Warehouse.Api.IntegrationTests.Fakes;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
private const string DatabaseName = "WarehouseIntegrationTests";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<WarehouseDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<IDbContextFactory<WarehouseDbContext>>();
            services.RemoveAll<WarehouseDbContext>();

            services.AddDbContextFactory<WarehouseDbContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });

            services.RemoveAll<ICacheService>();
            services.AddSingleton<ICacheService, TestCacheService>();

            services.RemoveAll<IFileStorageService>();
            services.AddSingleton<TestFileStorageService>();
            services.AddSingleton<IFileStorageService>(serviceProvider =>
                serviceProvider.GetRequiredService<TestFileStorageService>());

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        TestAuthHandler.SchemeName;

                    options.DefaultChallengeScheme =
                        TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    _ => { });
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

    private static void ResetDatabase(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<WarehouseDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var fileStorage = scope.ServiceProvider
            .GetRequiredService<TestFileStorageService>();

        fileStorage.Reset();

        SeedDatabase(context);
    }

    private static void SeedDatabase(WarehouseDbContext context)
    {
        var supplier = new Supplier(
            "sup1",
            "lebanon",
            "sup1@mail.com",
            "+12-345-111");

        var products = new[]
        {
            new Product(
                "keyboard2",
                "KEY-002",
                "budget keyboard",
                20,
                25,
                supplier,
                DateTime.UtcNow.AddYears(2)),

            new Product(
                "mouse1",
                "MOU-001",
                "budget mouse",
                10,
                5,
                supplier,
                DateTime.UtcNow.AddYears(2))
        };

        context.Suppliers.Add(supplier);
        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
tests/Warehouse.Api.IntegrationTests/Products/ProductsEndpointTests.cs
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.IntegrationTests.Fakes;
using Warehouse.Application.Commands.Products.AddProductImage;
using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Application.Queries.Products.GetProductImages;
using Warehouse.Application.ViewModels;
using Warehouse.Presentation.Models;

namespace Warehouse.Api.IntegrationTests.Products;

public class ProductsEndpointTests :
IClassFixture<CustomWebApplicationFactory>,
IAsyncLifetime
{
private readonly CustomWebApplicationFactory _factory;
private readonly HttpClient _client;

    public ProductsEndpointTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
    }

    public Task InitializeAsync()
    {
        _factory.ResetDatabase();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAllProducts_ReturnsSeededProducts()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await response.Content
            .ReadFromJsonAsync<List<ProductViewModel>>();

        products.Should().NotBeNull();
        products!.Should().HaveCount(2);
        products.Should().Contain(product =>
            product.SKU == "KEY-002");
    }

    [Fact]
    public async Task GetProductById_ExistingProduct_ReturnsProduct()
    {
        var seededProduct =
            await GetSeededProductAsync("KEY-002");

        var response = await _client.GetAsync(
            $"/api/products/{seededProduct.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await response.Content
            .ReadFromJsonAsync<ProductViewModel>();

        product.Should().NotBeNull();
        product!.Id.Should().Be(seededProduct.Id);
        product.SKU.Should().Be("KEY-002");
    }

    [Fact]
    public async Task GetProductById_InvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            $"/api/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content
            .ReadFromJsonAsync<ApiErrorResponse>();

        error.Should().NotBeNull();
        error!.Code.Should().Be("NOT_FOUND");
    }

    [Fact]
    public async Task SearchProducts_NameFilter_ReturnsMatches()
    {
        var response = await _client.GetAsync(
            "/api/products/search?name=keyboard2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await response.Content
            .ReadFromJsonAsync<List<ProductViewModel>>();

        products.Should().NotBeNull();
        products!.Should().ContainSingle();
        products[0].SKU.Should().Be("KEY-002");
    }

    [Fact]
    public async Task GetLowStockProducts_ReturnsLowStockProducts()
    {
        var response = await _client.GetAsync(
            "/api/products/low-stock");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await response.Content
            .ReadFromJsonAsync<List<ProductViewModel>>();

        products.Should().NotBeNull();

        products!.Should().ContainSingle(product =>
            product.SKU == "MOU-001");

        products.Should().OnlyContain(product =>
            product.QuantityInStock < 10 &&
            !product.IsArchived);
    }

    [Fact]
    public async Task CreateProduct_ValidRequest_PersistsCompleteProduct()
    {
        var request = CreateValidProductRequest("MON-001");

        var response = await _client.PostAsJsonAsync(
            "/api/products",
            request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Content.Headers.ContentType!.MediaType.Should()
            .Be("application/json");

        var createdProduct = await response.Content
            .ReadFromJsonAsync<ProductViewModel>();

        createdProduct.Should().NotBeNull();
        createdProduct!.Id.Should().NotBe(Guid.Empty);
        createdProduct.Name.Should().Be(request.Name);
        createdProduct.SKU.Should().Be(request.SKU);
        createdProduct.Description.Should().Be(request.Description);
        createdProduct.Price.Should().Be(request.Price);

        createdProduct.QuantityInStock.Should()
            .Be(request.QuantityInStock);

        createdProduct.SupplierName.Should()
            .Be(request.SupplierName);

        createdProduct.ExpiryDate.Should()
            .Be(request.ExpiryDate);

        createdProduct.IsArchived.Should().BeFalse();
        createdProduct.CreatedAt.Should().NotBe(default);
        createdProduct.LastUpdatedAt.Should().NotBe(default);

        response.Headers.Location.Should().NotBeNull();

        response.Headers.Location!.ToString().Should()
            .EndWith($"/api/products/{createdProduct.Id}");

        var getResponse = await _client.GetAsync(
            $"/api/products/{createdProduct.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        getResponse.Content.Headers.ContentType!.MediaType.Should()
            .Be("application/json");

        var persistedProduct = await getResponse.Content
            .ReadFromJsonAsync<ProductViewModel>();

        persistedProduct.Should().BeEquivalentTo(createdProduct);
    }

    [Fact]
    public async Task UploadProductImage_ValidJpeg_PersistsImageAndBinaryContent()
    {
        var product = await GetSeededProductAsync("KEY-002");

        var imageBytes = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0,
            0x10, 0x20, 0x30, 0x40,
            0xFF, 0xD9
        };

        using var form = CreateImageForm(
            imageBytes,
            "keyboard.jpg",
            "image/jpeg");

        var response = await _client.PostAsync(
            $"/api/products/{product.Id}/image",
            form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Content.Headers.ContentType!.MediaType.Should()
            .Be("application/json");

        var result = await response.Content
            .ReadFromJsonAsync<AddProductImageResponse>();

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Id.Should().NotBe(Guid.Empty);

        var imagesResponse = await _client.GetAsync(
            $"/api/products/{product.Id}/images");

        imagesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        imagesResponse.Content.Headers.ContentType!.MediaType.Should()
            .Be("application/json");

        var images = await imagesResponse.Content
            .ReadFromJsonAsync<List<GetProductImagesResponse>>();

        images.Should().ContainSingle(image =>
            image.Id == result.Id &&
            image.FileName == "keyboard.jpg");

        var fileStorage = _factory.Services
            .GetRequiredService<TestFileStorageService>();

        fileStorage.Files.Should().ContainSingle();

        var storedFile = fileStorage.Files.Single();

        storedFile.Key.Should()
            .StartWith($"products/{product.Id}/")
            .And.EndWith(".jpg");

        storedFile.Value.ContentType.Should().Be("image/jpeg");

        storedFile.Value.FileSize.Should()
            .Be(imageBytes.LongLength);

        storedFile.Value.Content.Should().Equal(imageBytes);
    }

    [Fact]
    public async Task CreateProduct_DuplicateSku_ReturnsConflict()
    {
        var request = CreateValidProductRequest("KEY-002");

        var response = await _client.PostAsJsonAsync(
            "/api/products",
            request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await response.Content
            .ReadFromJsonAsync<ApiErrorResponse>();

        error.Should().NotBeNull();
        error!.Code.Should().Be("CONFLICT");
    }

    [Fact]
    public async Task UpdateQuantity_ExistingProduct_UpdatesQuantity()
    {
        var seededProduct =
            await GetSeededProductAsync("KEY-002");

        const int newQuantity = 40;

        var response = await _client.PostAsJsonAsync(
            $"/api/products/{seededProduct.Id}/quantity",
            newQuantity);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProduct =
            await _client.GetFromJsonAsync<ProductViewModel>(
                $"/api/products/{seededProduct.Id}");

        updatedProduct.Should().NotBeNull();

        updatedProduct!.QuantityInStock.Should()
            .Be(newQuantity);
    }

    [Fact]
    public async Task UpdatePrice_ExistingProduct_UpdatesPrice()
    {
        var seededProduct =
            await GetSeededProductAsync("KEY-002");

        const double newPrice = 35.50;

        var response = await _client.PostAsJsonAsync(
            $"/api/products/{seededProduct.Id}/price",
            newPrice);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProduct =
            await _client.GetFromJsonAsync<ProductViewModel>(
                $"/api/products/{seededProduct.Id}");

        updatedProduct.Should().NotBeNull();
        updatedProduct!.Price.Should().Be(newPrice);
    }

    [Fact]
    public async Task DeleteProduct_ExistingProduct_ArchivesProductAndKeepsImage()
    {
        var seededProduct =
            await GetSeededProductAsync("KEY-002");

        var imageBytes = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xD9
        };

        using var form = CreateImageForm(
            imageBytes,
            "keyboard.jpg",
            "image/jpeg");

        var uploadResponse = await _client.PostAsync(
            $"/api/products/{seededProduct.Id}/image",
            form);

        uploadResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var uploadedImage = await uploadResponse.Content
            .ReadFromJsonAsync<AddProductImageResponse>();

        uploadedImage.Should().NotBeNull();
        uploadedImage!.Success.Should().BeTrue();

        var response = await _client.DeleteAsync(
            $"/api/products/{seededProduct.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().BeNull();

        (await response.Content.ReadAsStringAsync())
            .Should().BeEmpty();

        var getProductResponse = await _client.GetAsync(
            $"/api/products/{seededProduct.Id}");

        getProductResponse.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var archivedProduct = await getProductResponse.Content
            .ReadFromJsonAsync<ProductViewModel>();

        archivedProduct.Should().NotBeNull();
        archivedProduct!.Id.Should().Be(seededProduct.Id);
        archivedProduct.SKU.Should().Be(seededProduct.SKU);
        archivedProduct.IsArchived.Should().BeTrue();

        var imagesResponse = await _client.GetAsync(
            $"/api/products/{seededProduct.Id}/images");

        imagesResponse.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        imagesResponse.Content.Headers.ContentType!.MediaType.Should()
            .Be("application/json");

        var persistedImages = await imagesResponse.Content
            .ReadFromJsonAsync<List<GetProductImagesResponse>>();

        persistedImages.Should().ContainSingle(image =>
            image.Id == uploadedImage.Id &&
            image.FileName == "keyboard.jpg");

        var fileStorage = _factory.Services
            .GetRequiredService<TestFileStorageService>();

        fileStorage.Files.Should().ContainSingle();

        fileStorage.Files.Single().Value.Content.Should()
            .Equal(imageBytes);
    }

    private async Task<ProductViewModel> GetSeededProductAsync(
        string sku)
    {
        var products =
            await _client.GetFromJsonAsync<List<ProductViewModel>>(
                "/api/products");

        return products!.Single(product =>
            product.SKU == sku);
    }

    private static CreateProductRequest CreateValidProductRequest(
        string sku)
    {
        return new CreateProductRequest
        {
            Name = "monitor1",
            SKU = sku,
            Description = "gaming monitor",
            Price = 150,
            QuantityInStock = 15,
            SupplierName = "sup1",
            ExpiryDate = DateTime.UtcNow.AddYears(2)
        };
    }

    private static MultipartFormDataContent CreateImageForm(
        byte[] imageBytes,
        string fileName,
        string contentType)
    {
        var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(imageBytes);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(contentType);

        form.Add(fileContent, "file", fileName);

        return form;
    }
}