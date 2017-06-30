using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.FileProviders;


namespace Collector.Common.Swagger.AspNetCore.Extensions
{
    public static class EmbeddedResourceAssemblyExtensions
    {
        public static string ReadEmbeddedFileAsString(this Assembly assembly, string filePath)
        {
            if (assembly == null)
            {
                return null;
            }
            var fileProvider = new EmbeddedFileProvider(assembly);
            var fileInfo = fileProvider.GetFileInfo(filePath);
            if (fileInfo.Exists)
            {
                using (var stream = new StreamReader(fileInfo.CreateReadStream(), Encoding.UTF8))
                {
                    var result = stream.ReadToEnd();
                    return result;
                }
            }
            return null;
        }

        public static MemoryStream ReadEmbeddedFileAsStream(this Assembly assembly, string filePath)
        {
            if (assembly == null)
            {
                return null;
            }
            var fileProvider = new EmbeddedFileProvider(assembly);
            var fileInfo = fileProvider.GetFileInfo(filePath);
            if (fileInfo.Exists)
            {
                using (var stream = fileInfo.CreateReadStream())
                {
                    var result = new MemoryStream();
                    stream.CopyTo(result);
                    result.Position = 0;
                    return result;
                }
            }
            return null;
        }

    }
}
