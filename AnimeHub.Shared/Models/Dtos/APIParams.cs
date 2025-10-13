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

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
