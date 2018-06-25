using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Collector.Common.Swagger.AspNetCore.Extensions.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ExtensionController : Controller
    {
        /// <summary>
        /// Returns a embedded resources as file content
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet(Constants.Route + "{path}")]
        public IActionResult GetEmbeddedResource(string path)
        {
            var assembly = typeof(ExtensionController).GetTypeInfo().Assembly;
            string contentType;

            var mapper = new FileExtensionContentTypeProvider();
            if (!mapper.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            // do not dispose this stream, otherwise the return File() method will fail
            var streamData = assembly.ReadEmbeddedFileAsStream(path);
            if (streamData != null)
            {
                return File(streamData, contentType);
            }
            return NotFound();
        }

        /// <summary>
        /// Returns a Collector Bank favicon (.png)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/swagger/images/favicon-32x32.png")]
        [HttpGet("/swagger/images/favicon-16x16.png")]
        [HttpGet("/swagger/favicon-32x32.png")]
        [HttpGet("/swagger/favicon-16x16.png")]
        [HttpGet("/images/favicon-32x32.png")]
        [HttpGet("/images/favicon-16x16.png")]
        public IActionResult GetFavicon()
        {
            return GetImage("Resources.favicon.png");
        }

        /// <summary>
        /// Returns a Collector Bank Logo
        /// </summary>
        /// <returns></returns>
        [HttpGet("/swagger/images/logo_small.png")]
        public IActionResult GetLogo()
        {
            return GetImage("Resources.collector-bank250x26.png");
        }

        private IActionResult GetImage(string fileName)
        {
            var assembly = typeof(ExtensionController).GetTypeInfo().Assembly;

            // do not dispose this stream, otherwise the return File() method will fail
            var streamData = assembly.ReadEmbeddedFileAsStream(fileName);
            return File(streamData, "images/apng");
        }
    }
}