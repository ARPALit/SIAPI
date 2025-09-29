using System;
using System.Collections.Generic;

namespace Arpal.SiApi.WebApplication.Database;

public partial class SiapiServizi
{
    public decimal IdServizio { get; set; }

    public string? DescServizio { get; set; }

    public string? SqlStatement { get; set; }

    public DateTime? DataIni { get; set; }

    public DateTime? DataFin { get; set; }

    public string NomeServizio { get; set; } = null!;

    public string? AuthRequired { get; set; }

    public string ApikeyRequired { get; set; } = null!;

    public string? HelpServizio { get; set; }

    public string FiltroServizio { get; set; } = null!;

    public virtual ICollection<SiapiQuerylog> SiapiQuerylogs { get; set; } = new List<SiapiQuerylog>();

    public virtual ICollection<SiapiServiziApikey> SiapiServiziApikeys { get; set; } = new List<SiapiServiziApikey>();

    public virtual ICollection<SiapiServiziParametri> SiapiServiziParametris { get; set; } = new List<SiapiServiziParametri>();
}
