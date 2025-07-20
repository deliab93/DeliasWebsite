using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Web.Common.Controllers;

namespace DeliasWebsite.Core.Features.Seo
{
    public class RobotsTxtController : UmbracoPageController
    {
        public const string RoutePattern = "/robots.txt";

        private const string ProductionDefaultRobotsTxt =
                                @"User-agent: *
                            Disallow: /App_Plugins/
                            Disallow: /bin/
                            Disallow: /umbraco/
                            Disallow: /wwwroot/
                            Disallow: /views/
                            Disallow: /uSync/
                            Disallow: /smidge/
                            Disallow: /Properties/
                            Disallow: /obj/
                            Disallow: /*.axd
                            Disallow: /sitemap/";

        private const string NonProdDefaultRobotsTxt =
                                @"User-agent: *
                                Disallow: /";

        private readonly IWebHostEnvironment _hostEnvironment;

        public RobotsTxtController(
            ILogger<RobotsTxtController> logger,
            ICompositeViewEngine compositeViewEngine,
            IWebHostEnvironment hostEnvironment)
            : base(logger, compositeViewEngine)
        {
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return Content(_hostEnvironment.IsProduction() ? ProductionDefaultRobotsTxt : NonProdDefaultRobotsTxt, MediaTypeNames.Text.Plain);
        }
    }
}
