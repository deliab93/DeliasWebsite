using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace DeliasWebsite.Core.Features.Seo
{
    public sealed class SitemapXmlBuilder : ISitemapXmlBuilder
    {
        private static readonly XNamespace Ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
        private static readonly XNamespace NsXhtml = XNamespace.Get("http://www.w3.org/1999/xhtml");

        public XDocument Build(IPublishedContent content)
        {
            var urlset = new XElement(
                Ns + "urlset",
                new XAttribute(XNamespace.Xmlns + "xhtml", NsXhtml)
            );

            urlset.Add(CreateUrlElements(content));

            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                urlset
            );
        }

        private IEnumerable<XElement> CreateUrlElements(IPublishedContent content)
        {
            var url = new XElement(Ns + "url");
            url.Add(new XElement(Ns + "loc", content.Url(mode: UrlMode.Absolute)));
            url.Add(new XElement(Ns + "lastmod", content.UpdateDate.ToString("yyyy-MM-dd")));
            yield return url;

            var children = content.Children().Where(ShowInSiteMap);
            foreach (var child in children.SelectMany(CreateUrlElements))
                yield return child;
        }

        private static bool ShowInSiteMap(IPublishedContent content)
        {
            if (!content.IsPublished()) return false;
            if (!content.IsVisible()) return false;
            if (content.TemplateId is null or <= 0) return false;
            if (content.ContentType?.Alias == "contactSubmissionsFolder") return false;

            if (content.HasValue("hideFromSitemap"))
                return !content.Value<bool>("hideFromSitemap");

            return true;
        }
    }
}
