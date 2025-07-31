using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace DeliasWebsite.Core.Features.Contact
{
    public class ContactFormController : SurfaceController
    {
        private readonly IContentService _contentService;
        private readonly ILogger<ContactFormController> _logger;
        private readonly IEmailSender _emailSender;

        public ContactFormController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IContentService contentService,
            ILogger<ContactFormController> logger,
            IEmailSender emailSender
        ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _contentService = contentService;
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] 
        public async Task<IActionResult> Submit(ContactFormViewModel model)
        {
            bool isApiRequest = Request.Headers.ContainsKey("X-Worker-Request") ||
                                Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                if (isApiRequest)
                    return BadRequest(new { success = false, message = "Invalid form data" });

                TempData["scrollToForm"] = true;
                return CurrentUmbracoPage();
            }

            try
            {
                var rootNode = _contentService.GetRootContent().FirstOrDefault();
                if (rootNode == null)
                {
                    _logger.LogError("Root node not found.");
                    if (isApiRequest)
                        return StatusCode(500, new { success = false, message = "Site structure error." });

                    TempData["error"] = "Unable to process request.";
                    return RedirectToCurrentUmbracoPage();
                }

             
                var folderNode = _contentService.GetPagedChildren(rootNode.Id, 0, 100, out _)
                    .FirstOrDefault(x => x.ContentType.Alias == "contactSubmissionsFolder");

                if (folderNode == null)
                {
                    folderNode = _contentService.Create("Contact Submissions", rootNode.Id, "contactSubmissionsFolder");
                    _contentService.SaveAndPublish(folderNode);
                }

          
                var submissionName = $"Contact from {model.Name} - {DateTime.UtcNow:yyyy-MM-dd HH:mm}";
                var submission = _contentService.Create(submissionName, folderNode.Id, "contactForm");

                submission.SetValue("nameFrom", model.Name);
                submission.SetValue("email", model.Email);
                submission.SetValue("subject", model.Subject ?? string.Empty);
                submission.SetValue("message", model.Message);
                submission.SetValue("submissionDate", DateTime.UtcNow.ToString("u"));
                _contentService.SaveAndPublish(submission);

                // Send email
                var emailMessage = new EmailMessage(
                    from: "deliab93@icloud.com",
                    to: "deliab93@icloud.com",
                    subject: $"Contact Form: {model.Subject ?? "No Subject"}",
                    body: $"Name: {model.Name}\nEmail: {model.Email}\n\nMessage:\n{model.Message}",
                    false
                );

                await _emailSender.SendAsync(emailMessage, emailType: "ContactFormSubmission");

                if (isApiRequest)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Message sent successfully!"
                    });
                }

                TempData["success"] = "Message sent successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving contact form submission.");
                if (isApiRequest)
                    return StatusCode(500, new { success = false, message = "Something went wrong." });

                TempData["error"] = "Something went wrong. Please try again.";
            }

            TempData["scrollToForm"] = true;
            return RedirectToCurrentUmbracoPage();
        }
    }
}
