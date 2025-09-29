using System;
using System.Collections.Generic;

namespace Arpal.SiApi.WebApplication.Database;

public partial class SiapiServiziApikey
{
    public decimal? IdServizio { get; set; }

    public decimal IdApikey { get; set; }

    public DateTime? DataIni { get; set; }

    public DateTime? DataFin { get; set; }

    public decimal IdServizioApikey { get; set; }

    public virtual SiapiApikey IdApikeyNavigation { get; set; } = null!;

    public virtual SiapiServizi? IdServizioNavigation { get; set; }
}
