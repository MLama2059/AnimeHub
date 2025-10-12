using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AnimeHub.Shared.Models.Dtos
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // 1. Get the total count of ALL items (needed for TotalPages)
            var count = await source.CountAsync();

            // 2. Apply the paging logic: Skip the previous pages, Take the current page size
            var items = await source
                .Skip((pageNumber - 1) * pageSize) // Example: Page 2, Size 25 -> Skip (2-1)*25 = 25 items
                .Take(pageSize)
                .ToListAsync();

            // 3. Return the results wrapped in the PagedList metadata
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
