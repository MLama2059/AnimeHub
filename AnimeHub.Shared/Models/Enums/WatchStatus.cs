using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnimeHub.Shared.Models.Enums
{
    public enum WatchStatus
    {
        [Display(Name = "Plan To Watch")]
        PlanToWatch = 1,
        Watching = 2,
        Completed = 3,
        [Display(Name = "On Hold")]
        OnHold = 4,
        Dropped = 5
    }
}
