using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnimeHub.Shared.Models.Enums
{
    public enum Status
    {
        [Display(Name = "Not Yet Aired")]
        NotYetAired = 1,
        Airing = 2,
        Completed = 3,
        Hiatus = 4,
        Cancelled = 5,
    }
}
