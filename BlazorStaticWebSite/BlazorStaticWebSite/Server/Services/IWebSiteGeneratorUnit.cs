using System.Threading.Tasks;

namespace BlazorStaticWebSite.Server.Services
{
    public interface IWebSiteGeneratorUnit
    {
        Task<string> GenerateBlazorAppToStaticAsync(string exportPath);
        Task GenerateWebSiteToFilesystemAsync(string url, string filename);
    }
}