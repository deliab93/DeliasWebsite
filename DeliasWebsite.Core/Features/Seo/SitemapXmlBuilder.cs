using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;


namespace DeliasWebsite.Core.Features.Seo
{
    public sealed class SitemapXmlBuilder : ISitemapXmlBuilder
    {
        private static readonly XNamespace Namespace = XNamespace.Get("https://www.sitemaps.org/schemas/sitemap/0.9");

        private static readonly XNamespace XHtmlNamespace = XNamespace.Get("https://www.w3.org/1999/xhtml");

        public XDocument Build(IPublishedContent content)
        {
            var rootNode = new XElement(Namespace + "urlset", new XAttribute(XNamespace.Xmlns + "xhtml", XHtmlNamespace));
            rootNode.Add(CreateUrlElement(content));
            return new XDocument(rootNode);
        }

        private IEnumerable<XElement> CreateUrlElement(IPublishedContent content)
        {
         
            var element = new XElement(Namespace + "url");

            element.Add(new XElement(Namespace + "loc", content.Url(mode: UrlMode.Absolute)));
            element.Add(new XElement(Namespace + "lastmod", content.UpdateDate.ToString("yyyy-MM-dd")));


            yield return element;
            var children = content.Children().Where(x => ShowInSiteMap(x)).ToList();
            if (children == null)
            {
                yield break;
            }

            foreach (var n in children.SelectMany(CreateUrlElement))
            {
                yield return n;
            }
        }

        private bool ShowInSiteMap(IPublishedContent content)
        {
            if (!content.IsPublished())
            {
                return false;
            }

            if (!content.IsVisible())
            {
                return false;
            }

            if (content.TemplateId == null || content.TemplateId <= 0)
            {
                return false;
            }

            if (content.ContentType.Alias == "contactSubmissionsFolder")
            {
                return false;
            }

            if (content.HasValue("hideFromSitemp"))
            {
                return !content.Value<bool>("hideFromSitemp");
            }

            return true;
        }
    }

}
