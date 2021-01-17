using BlazorStaticWebSite.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorStaticWebSite.Server.Services
{
    public class WebSiteGeneratorUnit : IWebSiteGeneratorUnit
    {
        private readonly HttpClient http;

        public WebSiteGeneratorUnit(HttpClient http)
        {
            this.http = http;
        }

        public async Task<string> GenerateBlazorAppToStaticAsync(string exportPath)
        {
            try
            {
                if (!exportPath.EndsWith(@"\"))
                    exportPath += @"\";

                // All files delete at exportPath 
                ClearExportPath(exportPath);


                // Loockup all Blazor routes with "GenerateStatic" attribute
                List<string> relativePaths = new List<string>();
                List<Assembly> blazorAssemblies =new List<Assembly>() { typeof(BlazorStaticWebSite.Client.Program).Assembly };

                var componentTypes = blazorAssemblies.SelectMany(a => a.ExportedTypes.Where(t => typeof(IComponent).IsAssignableFrom(t)));
                foreach (var componentType in componentTypes)
                {
                    if (componentType.GetCustomAttribute(typeof(GenerateStaticPageAttribute)) != null)
                    {
                        var routeAttributes = componentType.GetCustomAttributes<RouteAttribute>(inherit: false);
                        relativePaths.AddRange(routeAttributes.Select(t => t.Template).ToArray());
                    }
                }

                foreach (var relPath in relativePaths)
                {
                    await GenerateWebSiteToFilesystemAsync("https://localhost:5001"+ relPath, exportPath.TrimEnd('\\')+ relPath.Replace("/","\\").TrimEnd('\\') + @"\\index.html");
                }                

                return "ok";
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }
        }

        private static void ClearExportPath(string exportPath)
        {
            if (File.Exists(exportPath + "index.html"))
                File.Delete(exportPath + "index.html");
            foreach (var item in Directory.GetDirectories(exportPath).ToList())
            {
                if (Directory.Exists(item))
                    Directory.Delete(item, true);
            }
        }

        public async Task GenerateWebSiteToFilesystemAsync(string url, string filename)
        {
            try
            {
                string content = await http.GetStringAsync(url + "?static=1");

                string filenameFull = Path.GetFullPath(filename);
                string path = Path.GetDirectoryName(filenameFull);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                await File.WriteAllTextAsync(filename, content);
            }
            catch (Exception ex)
            {
                Debug.Write($"Error by GenerateWebSiteToFilesystem({url},{filename})");
                throw new Exception($"Error by GenerateWebSiteToFilesystem({url},{filename})", ex);
            }
        }
    }
}
