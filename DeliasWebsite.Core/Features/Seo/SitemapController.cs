using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;

namespace DeliasWebsite.Core.Features.Seo
{
    public class SitemapController : UmbracoPageController
    {
        public const string RoutePattern = "/sitemap.xml";

        private readonly ISitemapXmlBuilder _sitemapXmlBuilder;
        private readonly UmbracoHelper _umbracoHelper;

        public SitemapController(ILogger<SitemapController> logger,
            ICompositeViewEngine compositeViewEngine,
            ISitemapXmlBuilder sitemapXmlBuilder,
            UmbracoHelper umbracoHelper)
            : base(logger, compositeViewEngine)
        {
            _sitemapXmlBuilder = sitemapXmlBuilder;
            _umbracoHelper = umbracoHelper;
        }

        [HttpGet]
        [Produces("application/xml")]
        public IActionResult Index()
        {
            var xmlDoc = _sitemapXmlBuilder.Build(_umbracoHelper?.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == "home"));
            using var stringWriter = new StringWriter();
            xmlDoc.Save(stringWriter, SaveOptions.None);
            return new ContentResult
            {
                ContentType = "application/xml",
                Content = stringWriter.ToString(),
                StatusCode = 200
            };
        }
    }
}