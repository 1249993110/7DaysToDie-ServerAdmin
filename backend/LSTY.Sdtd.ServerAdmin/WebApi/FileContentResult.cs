using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    /// <summary>
    /// File content result
    /// </summary>
    internal class FileContentResult : IHttpActionResult
    {
        private readonly byte[] _fileContents;
        private readonly string _contentType;

        public FileContentResult(byte[] fileContents, string contentType)
        {
            if (fileContents == null)
                throw new ArgumentNullException(nameof(fileContents));

            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            _fileContents = fileContents;
            _contentType = contentType;
        }

        public FileContentResult(string filePath, string contentType)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            _fileContents = System.IO.File.ReadAllBytes(filePath);
            _contentType = contentType;
        }

        /// <inheritdoc/>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(_fileContents);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
            return Task.FromResult(response);
        }
    }
}