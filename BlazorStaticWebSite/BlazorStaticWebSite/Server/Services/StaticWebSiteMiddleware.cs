using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorStaticWebSite.Server.Services
{
    public class StaticWebSiteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _hostingEnvironment;

        public StaticWebSiteMiddleware(RequestDelegate next, IHostingEnvironment hostingEnvironment)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            if (hostingEnvironment == null) throw new ArgumentNullException(nameof(hostingEnvironment));

            _next = next;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Request == null) throw new ArgumentNullException(nameof(context.Request));

            var baseUrl = context.Request.Path.Value.Substring(1).Replace("/", "\\");
            var destinationFile = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", baseUrl, "index.html");

            try
            {
                if (File.Exists(destinationFile))
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    var fileInfo = new FileInfo(destinationFile);
                    context.Response.ContentLength = fileInfo.Length;
                    var responseHeader = context.Response.GetTypedHeaders();
                    responseHeader.LastModified = File.GetLastWriteTime(destinationFile);

                    await context.Response.SendFileAsync(destinationFile);
                }
                else
                {
                    await _next(context);
                    return;
               }
            }
            finally
            {                               
            }
        }

      

        private async Task WriteBodyToDisk(string responseBody, string destinationFile)
        {
            using (FileStream fs = new FileStream(destinationFile, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                await sw.WriteAsync(responseBody);
            }
        }
    }
}
