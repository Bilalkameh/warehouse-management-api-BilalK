using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Application.ViewModels;
using Warehouse.Presentation.Models;

namespace Warehouse.Api.IntegrationTests.Products;

public class ProductsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductsEndpointTests(CustomWebApplicationFactory factory)
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
        var response =
            await _client.GetAsync("/api/products");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var products = await response.Content
            .ReadFromJsonAsync<List<ProductViewModel>>();

        products.Should().NotBeNull();
        products!.Should().HaveCount(2);

        products.Should().Contain(product => product.SKU == "KEY-002");
    }


    [Fact]
    public async Task GetProductById_ExistingProduct_ReturnsProduct()
    {
        var seededProduct =
            await GetSeededProductAsync("KEY-002");

        var response = await _client.GetAsync($"/api/products/{seededProduct.Id}");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var product = await response.Content
            .ReadFromJsonAsync<ProductViewModel>();

        product.Should().NotBeNull();
        product!.Id.Should().Be(seededProduct.Id);
        product.SKU.Should().Be("KEY-002");
    }


    [Fact]
    public async Task GetProductById_InvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);

        var error = await response.Content
            .ReadFromJsonAsync<ApiErrorResponse>();

        error.Should().NotBeNull();
        error!.Code.Should().Be("NOT_FOUND");
    }


    [Fact]
    public async Task SearchProducts_NameFilter_ReturnsMatches()
    {
        var response = await _client.GetAsync("/api/products/search?name=keyboard2");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var products = await response.Content
            .ReadFromJsonAsync<List<ProductViewModel>>();

        products.Should().NotBeNull();
        products!.Should().ContainSingle();
        products![0].SKU.Should().Be("KEY-002");
    }


    [Fact]
    public async Task GetLowStockProducts_ReturnsLowStockProducts()
    {
        var response = await _client.GetAsync("/api/products/low-stock");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var products = await response.Content
            .ReadFromJsonAsync<List<ProductViewModel>>();

        products.Should().NotBeNull();

        var lowStockProducts = products!;

        lowStockProducts.Should().ContainSingle(product => product.SKU == "MOU-001");
        lowStockProducts.Should().OnlyContain(product => product.QuantityInStock < 10 && !product.IsArchived);
    }


    [Fact]
    public async Task CreateProduct_ValidRequest_ReturnsCreated()
    {
        var request = CreateValidProductRequest("MON-001");
        var response = await _client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var product = await response.Content
            .ReadFromJsonAsync<ProductViewModel>();

        product.Should().NotBeNull();
        product!.Id.Should().NotBe(Guid.Empty);
        product.SKU.Should().Be(request.SKU);
    }


    [Fact]
    public async Task CreateProduct_DuplicateSku_ReturnsConflict()
    {
        var request = CreateValidProductRequest("KEY-002");

        var response = await _client.PostAsJsonAsync("/api/products",request);

        response.StatusCode.Should()
            .Be(HttpStatusCode.Conflict);

        var error = await response.Content
            .ReadFromJsonAsync<ApiErrorResponse>();

        error.Should().NotBeNull();
        error!.Code.Should().Be("CONFLICT");
    }


    [Fact]
    public async Task UpdateQuantity_ExistingProduct_UpdatesQuantity()
    {
        var seededProduct = await GetSeededProductAsync("KEY-002");

        const int newQuantity = 40;

        var response = await _client.PostAsJsonAsync($"/api/products/{seededProduct.Id}/quantity", newQuantity);

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var updatedProduct = await _client
            .GetFromJsonAsync<ProductViewModel>($"/api/products/{seededProduct.Id}");

        updatedProduct.Should().NotBeNull();

        updatedProduct!.QuantityInStock.Should()
            .Be(newQuantity);
    }


    [Fact]
    public async Task UpdatePrice_ExistingProduct_UpdatesPrice()
    {
        var seededProduct = await GetSeededProductAsync("KEY-002");

        const double newPrice = 35.50;

        var response = await _client.PostAsJsonAsync($"/api/products/{seededProduct.Id}/price", newPrice);

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var updatedProduct = await _client
            .GetFromJsonAsync<ProductViewModel>($"/api/products/{seededProduct.Id}");

        updatedProduct.Should().NotBeNull();
        updatedProduct!.Price.Should().Be(newPrice);
    }


    [Fact]
    public async Task DeleteProduct_ExistingProduct_ReturnsOk()
    {
        var seededProduct = await GetSeededProductAsync("KEY-002");

        var response = await _client.DeleteAsync($"/api/products/{seededProduct.Id}");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task DeleteProduct_ArchivedProductStillExists()
    {
        var seededProduct =
            await GetSeededProductAsync("KEY-002");

        await _client.DeleteAsync($"/api/products/{seededProduct.Id}");
        var response = await _client.GetAsync($"/api/products/{seededProduct.Id}");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var product = await response.Content
            .ReadFromJsonAsync<ProductViewModel>();

        product.Should().NotBeNull();
        product!.IsArchived.Should().BeTrue();
    }


    private async Task<ProductViewModel>
        GetSeededProductAsync(string sku)
    {
        var products = await _client.GetFromJsonAsync<List<ProductViewModel>>("/api/products");

        return products!.Single(product => product.SKU == sku);
    }

    private static CreateProductRequest
        CreateValidProductRequest(string sku)
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
}