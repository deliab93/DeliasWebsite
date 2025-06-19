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
        public async Task<IActionResult> Submit(string name, string userEmail, string message)
        {
            try
            {
                var submission = _contentService.Create(name, -1, "contactSubmission");

                submission.SetValue("name", name);
                submission.SetValue("email", userEmail);
                submission.SetValue("message", message);
                submission.SetValue("submissionDate", DateTime.UtcNow.ToString("u"));
                _contentService.SaveAndPublish(submission);

                var emailMessage = new EmailMessage(
                    from: userEmail,
                    to: "deliab93@icloud.com",
                    subject: "New Contact Form Submission",
                    body: $"Name: {name}\nEmail: {userEmail}\n\nMessage:\n{message}",
                    false
                );

                // Specify the emailType parameter to match the method signature
                await _emailSender.SendAsync(emailMessage, emailType: "ContactFormSubmission");

                TempData["success"] = "Message sent successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving contact form submission.");
                TempData["error"] = "Something went wrong. Please try again.";
            }

            return RedirectToCurrentUmbracoPage();
        }

    }
}
