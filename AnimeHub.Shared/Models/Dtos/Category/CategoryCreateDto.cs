using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.Category
{
    public class CategoryCreateDto
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
