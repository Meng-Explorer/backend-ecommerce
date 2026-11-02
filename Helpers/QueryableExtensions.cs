using Microsoft.EntityFrameworkCore;

namespace WepApi.Helpers;

public static class QueryableExtensions
    {
        public static async Task<PageResult<T>> ToPagedListAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);

            var totalRecords = await query.CountAsync();
            var skip = (long)(pageNumber - 1) * pageSize;

            List<T> data;
            if (skip >= totalRecords)
            {
                // Past the end of the result set - nothing to fetch.
                data = [];
            }
            else
            {
                var take = (int)Math.Min(skip + pageSize, int.MaxValue);
                var rows = await query.Take(take).ToListAsync();
                data = rows.Skip((int)skip).ToList();
            }

            return new PageResult<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords/(double)pageSize),
                Data = data
            };
        }   
    }
