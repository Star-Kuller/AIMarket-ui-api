using IAE.Microservice.Domain.Entities.Common;

namespace IAE.Microservice.Application.Common
{
    public abstract class PagedListQueryBase
    {
        public const int MaxPageSize = 5000;

        /// <remarks>
        /// Pagination starts at 0.
        /// </remarks>
        public int Page { get; set; }
        public int PageSize { get; set; } = MaxPageSize;
    }

    public class PagedListQuery : PagedListQueryBase
    {
        public string SearchTerm { get; set; }
        public Status? Status { get; set; }

        public long GetId() => long.TryParse(SearchTerm, out var number) ? number : default;
    }

    public class SortablePagedListQuery : PagedListQuery
    {
        public string SortBy { get; set; }
        public bool SortDesc { get; set; }
    }
}