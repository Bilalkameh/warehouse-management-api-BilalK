using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Warehouse.Api.UnitTests.Helpers;

public static class MultipartFormHelper
{
    public static IFormFile CreateFormFile(string fileName, string contentType, int fileSize = 3)
    {
        var stream = new MemoryStream(new byte[fileSize]);

        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    public static MultipartFormDataContent CreateMultipartContent(string fileName, string contentType, int fileSize = 3)
    {
        var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[fileSize]);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(fileContent, "file", fileName);

        return form;
    }
}