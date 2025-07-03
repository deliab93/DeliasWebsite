using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DeliasWebsite.Core.Features.Search
{
    public class SearchPageViewModel : ContentModel<IPublishedContent>
    {
        public SearchPageViewModel(IPublishedContent content) : base(content)
        { }
        public IEnumerable<SearchResultModel> Results { get; set; }
        public string Query { get; set; }
    }
}
