using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Warehouse.Api.IntegrationTests.Helpers;
using Warehouse.Application.Commands.Products.AddProductImage;
using Warehouse.Application.ViewModels;

namespace Warehouse.Api.IntegrationTests.Images;

public class ImageUploadEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ImageUploadEndpointTests(CustomWebApplicationFactory factory)
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


    [Theory]
    [InlineData("product.jpg", "image/jpeg")]
    [InlineData("product.png", "image/png")]
    public async Task UploadImage_ValidImage_ReturnsOk(string fileName, string contentType)
    {
        var product = await GetSeededProductAsync("KEY-002");

        using var form = MultipartFormHelper.CreateMultipartContent(fileName, contentType);

        var response = await _client.PostAsync($"/api/products/{product.Id}/image", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<AddProductImageResponse>();

        result.Should().NotBeNull();

        var uploadResult = result!;
        uploadResult.Success.Should().BeTrue();
        uploadResult.Id.Should().NotBe(Guid.Empty);
    }


    [Fact]
    public async Task UploadImage_TxtFile_ReturnsBadRequest()
    {
        using var form = MultipartFormHelper.CreateMultipartContent("product.txt", "text/plain");

        var response = await _client.PostAsync($"/api/products/{Guid.NewGuid()}/image", form);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var message = await response.Content.ReadAsStringAsync();
        message.Should().Contain("Only Jpeg and Png images are allowed.");
    }


    [Fact]
    public async Task UploadImage_OversizedFile_ReturnsBadRequest()
    {
        const int maxFileSize = 5 * 1024 * 1024;

        using var form = MultipartFormHelper.CreateMultipartContent("product.jpg", "image/jpeg", maxFileSize + 1);

        var response = await _client.PostAsync($"/api/products/{Guid.NewGuid()}/image", form);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var message = await response.Content.ReadAsStringAsync();
        message.Should().Contain("Image cannot exceed 5 MB.");
    }


    private async Task<ProductViewModel>
        GetSeededProductAsync(string sku)
    {
        var products = await _client
            .GetFromJsonAsync<List<ProductViewModel>>("/api/products");

        return products!.Single(product => product.SKU == sku);
    }
}