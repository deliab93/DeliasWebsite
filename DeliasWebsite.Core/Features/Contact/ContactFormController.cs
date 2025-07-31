using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
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
            var rootNode = _contentService.GetRootContent().FirstOrDefault();
            if (rootNode == null)
            {
                _logger.LogError("Root node not found.");
                TempData["error"] = "Unable to process request right now.";
                return RedirectToCurrentUmbracoPage();
            }

            var folderNode = _contentService.GetPagedChildren(rootNode.Id, 0, 100, out var total)
                           .FirstOrDefault(x => x.ContentType.Alias == "contactSubmissionsFolder");

            if (folderNode == null)
            {
                _logger.LogError("Folder node of type 'contactSubmissionsFolder' not found.");
                TempData["error"] = "Unable to submit your message at this time.";
                TempData["scrollToForm"] = true;
                return RedirectToCurrentUmbracoPage();
            }

            int submissionsParentId = folderNode.Id;

            if (!ModelState.IsValid)
            {
              
                TempData["scrollToForm"] = true;
                return CurrentUmbracoPage();
            }

            try
            {
                var submissionName = $"Contact from {model.Name} - {DateTime.UtcNow:yyyy-MM-dd HH:mm}";
                var submission = _contentService.Create(submissionName, submissionsParentId, "contactForm");

                submission.SetValue("nameFrom", model.Name);
                submission.SetValue("email", model.Email);
                submission.SetValue("subject", model.Subject ?? string.Empty);
                submission.SetValue("message", model.Message);
                submission.SetValue("submissionDate", DateTime.UtcNow.ToString("u"));
                _contentService.SaveAndPublish(submission);

                var emailMessage = new EmailMessage(
                    from: "deliab93@icloud.com",
                    to: "deliab93@icloud.com",
                    subject: $"Contact Form: {model.Subject ?? "No Subject"}",
                    body: $"Name: {model.Name}\nEmail: {model.Email}\n\nMessage:\n{model.Message}",
                    false
                );

                await _emailSender.SendAsync(emailMessage, emailType: "ContactFormSubmission");

                TempData["success"] = "Message sent successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving contact form submission.");
                TempData["scrollToForm"] = true;
                TempData["error"] = "Something went wrong. Please try again.";
            }

            TempData["scrollToForm"] = true;
            return RedirectToCurrentUmbracoPage();
        }

    }
}
