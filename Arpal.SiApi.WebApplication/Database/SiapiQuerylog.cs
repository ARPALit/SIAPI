using System;
using System.Collections.Generic;

namespace Arpal.SiApi.WebApplication.Database;

public partial class SiapiQuerylog
{
    public decimal IdQuerylogs { get; set; }

    public DateTime QueryDate { get; set; }

    public decimal IdApikey { get; set; }

    public decimal IdServizio { get; set; }

    public string? Json { get; set; }

    public virtual SiapiApikey IdApikeyNavigation { get; set; } = null!;

    public virtual SiapiServizi IdServizioNavigation { get; set; } = null!;
}
