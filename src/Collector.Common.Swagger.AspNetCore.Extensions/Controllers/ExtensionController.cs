using System.Reflection;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetEmbeddedResource(string path)
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
        [HttpGet("/favicon.ico")]
        [HttpGet("/swagger/images/favicon-32x32.png")]
        [HttpGet("/swagger/images/favicon-16x16.png")]
        public async Task<IActionResult> GetFavicon()
        {
            var assembly = typeof(ExtensionController).GetTypeInfo().Assembly;

            var streamData = assembly.ReadEmbeddedFileAsStream("Resources.favicon.ico");
            return File(streamData, "image/x-icon");
        }
    }
}