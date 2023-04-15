using ASPNETCoreLargeFileUploadExamples;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = null;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(600);
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MultipartHeadersCountLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

var app = builder.Build();

app.MapGet("/", () => Results.Extensions.Html(PageHtml.IndexHtml));
app.MapGet("/uploadcomplete", () => "Upload Complete");

app.MapPost("/upload-iformfile", async ([FromForm] IFormFile file) =>
{
    var path = Path.Combine(@"C:\Temp", Path.GetRandomFileName());
    await using var stream = File.Create(path);

    await file.CopyToAsync(stream);

    return Results.Redirect("/uploadcomplete");
});

app.MapPost("/upload-bodyreader", async (HttpRequest request) =>
{
    var reader = request.BodyReader;

    var path = Path.Combine(@"C:\Temp", Path.GetRandomFileName());
    //await using var stream = File.Create(path);

    while (true)
    {
        var result = await reader.ReadAsync();
        var buffer = result.Buffer;

        //await stream.WriteAsync(buffer.ToArray());
        //await stream.FlushAsync();

        reader.AdvanceTo(buffer.Start, buffer.End);

        if (result.IsCompleted)
        {
            break;
        }
    }

    return Results.Redirect("/UploadComplete");
});

app.MapPost("/upload-multipartreader", async (HttpRequest request) =>
{
    var formOptions = new FormOptions();
    var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(request.ContentType), formOptions.MultipartBoundaryLengthLimit);
    var reader = new MultipartReader(boundary, request.Body);
    var section = await reader.ReadNextSectionAsync();

    while (section != null)
    {
        var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

        if (hasContentDispositionHeader)
        {
            var path = Path.Combine(@"C:\Temp", Path.GetRandomFileName());
            await using var fileStream = File.Create(path);

            await using var memoryStream = new MemoryStream();
            await section.Body.CopyToAsync(memoryStream);

            await fileStream.WriteAsync(memoryStream.ToArray());
        }

        section = await reader.ReadNextSectionAsync();
    }

    return Results.Redirect("/uploadcomplete");
});

Console.WriteLine($"Process ID: {Environment.ProcessId}");
app.Run();
