using Umbraco.Cms.Core.Mail; // Ensure this namespace is included
using Microsoft.Extensions.Options; // Add this namespace if not already present
using Umbraco.Cms.Core.Configuration.Models; // Add this namespace for EmailSenderOptions

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//builder.Services.Configure<EmailSenderOptions>(
//    builder.Configuration.GetSection("Smtp"));

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

app.UseHttpsRedirection();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
