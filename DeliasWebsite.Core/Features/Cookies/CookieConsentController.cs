using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.AspNetCore.Http;

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

        [HttpPost]
        [Route(RoutePattern + "/accept")]
        [IgnoreAntiforgeryToken]
        public IActionResult Accept()
        {
            Response.Cookies.Append("CookieConsent", "accepted", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });

            return NoContent();
        }

        [HttpPost]
        [Route(RoutePattern + "/decline")]
        [IgnoreAntiforgeryToken]
        public IActionResult Decline()
        {
            Response.Cookies.Append("CookieConsent", "declined", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });

            return NoContent();
        }
    }
}
