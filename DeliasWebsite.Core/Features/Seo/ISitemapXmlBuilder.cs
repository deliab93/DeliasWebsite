using System.Xml.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DeliasWebsite.Core.Features.Seo
{
    public interface ISitemapXmlBuilder
    {
        XDocument Build(IPublishedContent content);
    }
}