namespace ASPNETCoreLargeFileUploadExamples
{
    public static class PageHtml
    {
        public static string IndexHtml = """
            <!DOCTYPE html>
            <html>
            <head>
            </head>
            <body>
                <p>IFormFile</p>
            	<form id="upload-iformfile" action="/upload-iformfile" enctype="multipart/form-data" method="post">
                    <input id="file" name="file" type="file">
                    <button type="submit">Upload</button>
                </form>
                <br>

                <p>BodyReader</p>
            	<form id="upload-bodyreader" action="/upload-bodyreader" enctype="multipart/form-data" method="post">
                    <input id="file" name="file" type="file">
                    <button type="submit">Upload</button>
                </form>
                <br>

                <p>MultipartReader</p>
            	<form id="upload-multipartreader" action="/upload-multipartreader" enctype="multipart/form-data" method="post">
                    <input id="file" name="file" type="file">
                    <button type="submit">Upload</button>
                </form>
                <br>
            </body>
            </html>
            """;
    }
}
