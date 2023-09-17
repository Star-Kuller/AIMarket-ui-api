using System.Collections.Generic;

namespace IAE.Microservice.Application.Common
{
    public abstract class PagedResult
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
    }

    public class PagedResult<T> : PagedResult where T : class
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}