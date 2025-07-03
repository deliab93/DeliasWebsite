using Examine;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace DeliasWebsite.Core.Features.Search
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public SearchService(IExamineManager examineManager, IHttpContextAccessor contextAccessor, IUmbracoContextFactory umbracoContextFactory)
        {
            _examineManager = examineManager;
            _umbracoContextFactory = umbracoContextFactory;
        }
        public IEnumerable<IPublishedContent> GetSearchResults(string? query = "", string? sortOrder = "relevance", int pageNumber = 1, int pageSize = 4)
        {
            IEnumerable<IPublishedContent> result = Enumerable.Empty<IPublishedContent>();

            IEnumerable<string> ids = Array.Empty<string>();
            if (!string.IsNullOrEmpty(query) && _examineManager.TryGetIndex("ExternalIndex", out IIndex? index))
            {
                var results = index.Searcher.Search(query);
                if (results?.Any() ?? false)
                {
                    ids = results.Select(x => x.Id);
                }
            }

            using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                IPublishedContentCache? contentCache = umbracoContextReference?.UmbracoContext?.Content;
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int contentId))
                    {
                        var content = contentCache?.GetById(contentId);
                        if (content != null && !content.Value<bool>("HideFromSearch"))
                        {
                            result = result.Append(content);
                        }
                    }
                }
            }

            if (sortOrder == "date")
            {
                result = result.OrderByDescending(x => x.UpdateDate);
            }

            return result;
        }
    }
}
