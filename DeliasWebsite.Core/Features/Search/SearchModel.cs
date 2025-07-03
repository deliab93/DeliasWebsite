using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliasWebsite.Core.Features.Search
{
    public class SearchModel
    {
        public string? Query { get; set; }
        public string? Page { get; set; }
        public string[]? TagQuery { get; set; }
        public string[]? OrgQuery { get; set; }
        public string? SortOrder { get; set; }
    }
}
