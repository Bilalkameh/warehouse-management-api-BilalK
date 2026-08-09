using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Warehouse.Api.IntegrationTests.Helpers;
using Warehouse.Application.Commands.Products.AddProductImage;
using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Application.Commands.Suppliers.CreateSupplier;
using Warehouse.Application.ViewModels;

namespace Warehouse.Api.IntegrationTests;

public class FullBusinessFlowTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public FullBusinessFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
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
    public async Task FullBusinessFlow_ValidRequests_CompletesSuccessfully()
    {
        //1. Create supplier
        var supplierRequest = new CreateSupplierRequest
        {
            Name = "fakesup1",
            Country = "lebanon",
            ContactEmail = "fakesupplier@mail.com",
            PhoneNumber = "+12-345-333"
        };

        var supplierResponse = await _client.PostAsJsonAsync("/api/suppliers", supplierRequest);
        supplierResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var supplier = await supplierResponse.Content
            .ReadFromJsonAsync<SupplierViewModel>();

        supplier.Should().NotBeNull();

        var createdSupplier = supplier!;

        createdSupplier.Id.Should().NotBe(Guid.Empty);

        // 2.Create product
        var productRequest = new CreateProductRequest
        {
            Name = "bilal",
            SKU = "BILAL-001",
            Description = "product for testing",
            Price = 100,
            QuantityInStock = 10,
            SupplierName = "sup1",
            ExpiryDate = DateTime.UtcNow.AddYears(2)
        };

        var productResponse = await _client.PostAsJsonAsync("/api/products", productRequest);

        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var product = await productResponse.Content
            .ReadFromJsonAsync<ProductViewModel>();

        product.Should().NotBeNull();
        var createdProduct = product!;
        createdProduct.Id.Should().NotBe(Guid.Empty);
        
        //3. assign the newly created supplier

        var assignResponse = await _client.PostAsync($"/api/products/{createdProduct.Id}" +
                                                     $"/assign-supplier/{createdSupplier.Id}", null);

        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        //4. upload image
        using var form = MultipartFormHelper.CreateMultipartContent("flow-product.jpg", "image/jpeg");

        var imageResponse = await _client.PostAsync($"/api/products/{createdProduct.Id}/image", form);

        imageResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var imageResult = await imageResponse.Content
            .ReadFromJsonAsync<AddProductImageResponse>();

        imageResult.Should().NotBeNull();
        imageResult!.Success.Should().BeTrue();
        imageResult.Id.Should().NotBe(Guid.Empty);

        //5. update product qty
        const int updatedQuantity = 30;

        var quantityResponse = await _client.PostAsJsonAsync(
            $"/api/products/{createdProduct.Id}/quantity",
            updatedQuantity);

        quantityResponse.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        //6. update price
        const double updatedPrice = 125;

        var priceResponse = await _client.PostAsJsonAsync($"/api/products/{createdProduct.Id}/price", updatedPrice);

        priceResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        //7. archive product
        var archiveResponse = await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 8. verify final state
        var verifyResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");

        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var finalProduct = await verifyResponse.Content
            .ReadFromJsonAsync<ProductViewModel>();

        finalProduct.Should().NotBeNull();

        var archivedProduct = finalProduct!;

        archivedProduct.Id.Should().Be(createdProduct.Id);
        archivedProduct.SupplierName.Should().Be(createdSupplier.Name);
        archivedProduct.QuantityInStock.Should().Be(updatedQuantity);
        archivedProduct.Price.Should().Be(updatedPrice);
        archivedProduct.IsArchived.Should().BeTrue();
    }
}