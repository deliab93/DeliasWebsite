using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DeliasWebsite.Core.Features.Search
{
    public interface ISearchService
    {
        IEnumerable<IPublishedContent> GetSearchResults(string? query = "", string? sortOrder = "relevance", int pageNumber = 1, int pageSize = 10);
    }
}
