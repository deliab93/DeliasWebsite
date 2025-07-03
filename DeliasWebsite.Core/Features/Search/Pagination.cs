using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliasWebsite.Core.Features.Search
{
    public class Pagination
    {
        public Pagination(int totalRecords = 0, int pageSize = 0, int currentPage = 0, string pageType = "", string query = "", string tagFilter = "")
        {
            TotalRecords = totalRecords;
            PageSize = pageSize > 0 ? pageSize : 10;
            CurrentPage = currentPage >= 1 ? currentPage : 1;
            TagFilter = tagFilter;
            PageType = pageType;
            Query = query;

            int paginationItems = 4;
            int offset = (int)Math.Ceiling((decimal)paginationItems / 2);

            TotalPages = (int)Math.Ceiling((decimal)TotalRecords / PageSize);
            CurrentPage = Math.Max(1, Math.Min(TotalPages, CurrentPage));
            PaginationStart = CurrentPage - offset;
            PaginationEnd = CurrentPage + offset;

            if (TotalPages <= paginationItems)
            {
                PaginationStart = 1;
                PaginationEnd = TotalPages;
            }
            else if (CurrentPage <= offset)
            {
                PaginationStart = 1;
                PaginationEnd = paginationItems;
            }
            else if (CurrentPage + offset >= TotalPages)
            {
                PaginationStart = TotalPages - paginationItems;
                PaginationEnd = TotalPages;
            }

            int fromCount = Skip + 1;
            int toCount = CurrentPage * PageSize;

            if (TotalRecords < PageSize)
            {
                PageSize = TotalRecords;
            }

            if (toCount >= TotalRecords)
            {
                toCount = TotalRecords;
            }

            Skip = PageSize * (CurrentPage - 1);
        }

        public int Skip { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PaginationStart { get; set; }
        public int PaginationEnd { get; set; }
        public int PageSize { get; set; }
        public string TagFilter { get; set; }
        public string PageType { get; set; }
        public string Query { get; set; }

        public int FromPageSummaryIndex => ((CurrentPage - 1) * PageSize) + 1;

        public int ToPageSummaryIndex =>
            TotalRecords - Skip >= PageSize
                ? ((CurrentPage - 1) * PageSize) + PageSize
                : ((CurrentPage - 1) * PageSize) + (TotalRecords - Skip);
    }
}
