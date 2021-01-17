using BlazorStaticWebSite.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorStaticWebSite.Server.Controllers
{
    [Route("[controller]/[action]")]
    public class WebSiteGeneratorController : Controller
    {
        private readonly IWebSiteGeneratorUnit generator;

        public WebSiteGeneratorController(IWebSiteGeneratorUnit generator)
        {
            this.generator = generator;
        }

        public async Task<string> Generate()
        {
            try
            {
                await generator.GenerateBlazorAppToStaticAsync(@".\wwwroot");                

                return "ok";
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }
            
        }
    }
}
