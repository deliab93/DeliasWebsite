using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.Common.Controllers;

namespace DeliasWebsite.Core.Features.Cookies
{
    public class CookieConsentController : UmbracoPageController
    {
        public const string RoutePattern = "/cookie-consent";

        public CookieConsentController(ILogger<CookieConsentController> logger, ICompositeViewEngine compositeViewEngine)
            : base(logger, compositeViewEngine)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/CookieConsent.cshtml");
        }
    }
}
