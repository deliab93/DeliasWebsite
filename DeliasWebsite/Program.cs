using DeliasWebsite.Core.Features.Contact;
using DeliasWebsite.Core.Features.Search;
using DeliasWebsite.Core.Features.Seo;
using DeliasWebsite.Core.Features.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using SixLabors.ImageSharp.Web.DependencyInjection;
using Umbraco.Cms.Core.Services;
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

builder.Services.AddImageSharp(options =>
{
    options.BrowserMaxAge = TimeSpan.FromDays(30);
    options.CacheMaxAge = TimeSpan.FromDays(365);
    options.BrowserMaxAge = TimeSpan.FromDays(30);


    options.OnParseCommandsAsync = context =>
    {
        var path = context.Context.Request.Path.Value ?? string.Empty;

        if (!path.StartsWith("/media", StringComparison.OrdinalIgnoreCase))
        {
            context.Commands.Clear(); 
        }

        return Task.CompletedTask;
    };
});

builder.Services.Configure<UmbracoRequestOptions>(options =>
{
    string[] allowList = new[] { "/sitemap.xml", "/robots.txt", CookieConsentController.RoutePattern };
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
        u.EndpointRouteBuilder.MapControllerRoute(
            nameof(CookieConsentController),
            CookieConsentController.RoutePattern,
            new
            {
                Controller = ControllerExtensions.GetControllerName<CookieConsentController>(),
                Action = nameof(CookieConsentController.Index)
            });
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });
app.UseImageSharp();


await app.RunAsync();
