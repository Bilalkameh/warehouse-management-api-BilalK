using System.Net.Http.Headers;

namespace Warehouse.Api.IntegrationTests.Helpers;

public static class MultipartFormHelper
{
    public static MultipartFormDataContent CreateMultipartContent(string fileName, string contentType,int fileSize = 3)
    {
        var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[fileSize]);
        
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(fileContent, "file", fileName);
        
        return form;
    }
}