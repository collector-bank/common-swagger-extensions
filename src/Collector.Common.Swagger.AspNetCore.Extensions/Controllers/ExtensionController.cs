using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Collector.Common.Swagger.AspNetCore.Extensions
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ExtensionController : Controller
    {
        /// <summary>
        /// Returns as embedded resources as file content
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet(Constants.Route + "{path}")]
        public IActionResult GetEmbeddedResource(string path)
        {
            var assembly = typeof(ExtensionController).GetTypeInfo().Assembly;
            var contentType = path.EndsWith(".js") ? "application/javascript" : "text/css";
            var streamData = assembly.ReadEmbeddedFileAsStream(path);
            if (streamData != null)
            {
                return File(streamData, contentType);
            }
            return NotFound();
        }

        /// <summary>
        /// Returns a Collector Bank favicon
        /// </summary>
        /// <returns></returns>
        [HttpGet("/swagger/images/favicon-32x32.png")]
        [HttpGet("/swagger/images/favicon-16x16.png")]
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

            var streamData = assembly.ReadEmbeddedFileAsStream(fileName);
            return File(streamData, "images/apng");
            
        }
    }
}