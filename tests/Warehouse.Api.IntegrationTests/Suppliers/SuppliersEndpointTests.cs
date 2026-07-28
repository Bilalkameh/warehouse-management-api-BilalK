using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Warehouse.Application.Commands.Suppliers.CreateSupplier;
using Warehouse.Application.ViewModels;

namespace Warehouse.Api.IntegrationTests.Suppliers;

public class SuppliersEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SuppliersEndpointTests(CustomWebApplicationFactory factory)
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
    public async Task CreateSupplier_ValidRequest_ReturnsCreated()
    {
        var request = CreateValidSupplierRequest("sup2");
        var response = await _client.PostAsJsonAsync("/api/suppliers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var supplier = await response.Content
            .ReadFromJsonAsync<SupplierViewModel>();

        supplier.Should().NotBeNull();

        var createdSupplier = supplier!;

        createdSupplier.Id.Should().NotBe(Guid.Empty);
        createdSupplier.Name.Should().Be(request.Name);
        createdSupplier.Country.Should().Be(request.Country);
        createdSupplier.ContactEmail.Should().Be(request.ContactEmail);
        createdSupplier.PhoneNumber.Should().Be(request.PhoneNumber);
        createdSupplier.IsActive.Should().BeTrue();
    }


    [Fact]
    public async Task GetSupplierById_ExistingSupplier_ReturnsSupplier()
    {
        var seededSupplier = await GetSeededSupplierAsync("sup1");
        var response = await _client.GetAsync($"/api/suppliers/{seededSupplier.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var supplier = await response.Content
            .ReadFromJsonAsync<SupplierViewModel>();

        supplier.Should().NotBeNull();

        supplier!.Id.Should().Be(seededSupplier.Id);
        supplier.Name.Should().Be("sup1");
    }


    [Fact]
    public async Task DeactivateSupplier_ExistingSupplier_DeactivatesSupplier()
    {
        var seededSupplier = await GetSeededSupplierAsync("sup1");
        var response = await _client.DeleteAsync($"/api/suppliers/{seededSupplier.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deactivatedSupplier = await _client
            .GetFromJsonAsync<SupplierViewModel>($"/api/suppliers/{seededSupplier.Id}");

        deactivatedSupplier.Should().NotBeNull();
        deactivatedSupplier!.IsActive.Should().BeFalse();
    }


    [Fact]
    public async Task AssignSupplier_ValidSupplier_AssignsSupplierToProduct()
    {
        var supplierRequest = CreateValidSupplierRequest("sup2");
        var createResponse = await _client.PostAsJsonAsync("/api/suppliers", supplierRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var newSupplier = await createResponse.Content.ReadFromJsonAsync<SupplierViewModel>();

        newSupplier.Should().NotBeNull();

        var seededProduct = await GetSeededProductAsync("KEY-002");

        var assignResponse = await _client
            .PostAsync($"/api/products/{seededProduct.Id}" + $"/assign-supplier/{newSupplier!.Id}", null);

        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProduct = await _client
            .GetFromJsonAsync<ProductViewModel>($"/api/products/{seededProduct.Id}");

        updatedProduct.Should().NotBeNull();

        updatedProduct!.SupplierName.Should().Be(newSupplier.Name);
    }


    private async Task<SupplierViewModel>
        GetSeededSupplierAsync(string name)
    {
        var suppliers = await _client
            .GetFromJsonAsync<List<SupplierViewModel>>("/api/suppliers");

        return suppliers!.Single(supplier => supplier.Name == name);
    }


    private async Task<ProductViewModel>
        GetSeededProductAsync(string sku)
    {
        var products = await _client
            .GetFromJsonAsync<List<ProductViewModel>>("/api/products");

        return products!.Single(product => product.SKU == sku);
    }


    private static CreateSupplierRequest
        CreateValidSupplierRequest(string name)
    {
        return new CreateSupplierRequest
        {
            Name = name,
            Country = "lebanon",
            ContactEmail = $"{name}@mail.com",
            PhoneNumber = "+12-345-222"
        };
    }
}