using DeliasWebsite.Core.Features.Contact;
using DeliasWebsite.Core.Features.Search;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models; 
using Umbraco.Cms.Core.Mail;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(ContactFormController).Assembly);
builder.Services.AddScoped<ISearchService, SearchService>();

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
