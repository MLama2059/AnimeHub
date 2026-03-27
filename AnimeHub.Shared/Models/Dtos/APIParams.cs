using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos
{
    public class APIParams
    {
        private const int MaxPageSize = 50; // Maximum allowed page size
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10; // Default page size
        public string? OrderBy { get; set; }
        public string? FilterOn { get; set; }
        public string? FilterQuery { get; set; }
        public int? StatusFilter { get; set; }

        // Filter properties
        public int? CategoryId { get; set; }
        public int? GenreId { get; set; }
        public int? Year { get; set; }
        public string? Season { get; set; }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
