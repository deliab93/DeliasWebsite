using DeliasWebsite.Core.Features.Contact;
using DeliasWebsite.Core.Features.Search;
using DeliasWebsite.Core.Features.Seo;
using Microsoft.AspNetCore.HttpOverrides;
using Umbraco.Cms.Web.Common.Routing;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .AddAzureBlobMediaFileSystem()
    .Build();

builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(ContactFormController).Assembly)
    .AddApplicationPart(typeof(SitemapController).Assembly);
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddSingleton<ISitemapXmlBuilder, SitemapXmlBuilder>();
builder.Services.Configure<UmbracoRequestOptions>(options =>
{
    string[] allowList = new[] { "/sitemap.xml", "/robots.txt" };
    options.HandleAsServerSideRequest = httpRequest =>
    {
        foreach (string route in allowList)
        {
            if (httpRequest.Path.StartsWithSegments(route))
            {
                return true;
            }
        }

        return false;
    };
});

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

//app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.EndpointRouteBuilder.MapControllerRoute(
            nameof(RobotsTxtController),
            RobotsTxtController.RoutePattern,
            new
            {
                Controller = ControllerExtensions.GetControllerName<RobotsTxtController>(),
                Action = nameof(RobotsTxtController.Index)
            });
        u.EndpointRouteBuilder.MapControllerRoute(
                      nameof(SitemapController),
                      SitemapController.RoutePattern,
                      new
                      {
                          Controller = ControllerExtensions.GetControllerName<SitemapController>(),
                          Action = nameof(SitemapController.Index)
                      });
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
