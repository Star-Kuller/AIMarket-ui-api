using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAE.Microservice.Application.Common;

namespace IAE.Microservice.Application.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<U>> GetPagedAsync<T, U>(this IQueryable<T> query,
            int page, int pageSize,
            IConfigurationProvider configurationProvider,
            CancellationToken cancellationToken = default(CancellationToken)) where U : class
        {
            var result = new PagedResult<U>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = page * pageSize;
            result.Results = await query.Skip(skip)
                .Take(pageSize)
                .ProjectTo<U>(configurationProvider)
                .AsSingleQuery()
                .ToListAsync(cancellationToken);
            return result;
        }
    }
}