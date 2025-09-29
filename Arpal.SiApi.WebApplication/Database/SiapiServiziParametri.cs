using System;
using System.Collections.Generic;

namespace Arpal.SiApi.WebApplication.Database;

public partial class SiapiServiziParametri
{
    public decimal IdServizioParametro { get; set; }

    public decimal IdServizio { get; set; }

    public string? UserField { get; set; }

    public string? FieldAlias { get; set; }

    public string? Datatype { get; set; }

    public string Mandatory { get; set; } = null!;

    public string? HelpParametro { get; set; }

    public virtual SiapiServizi IdServizioNavigation { get; set; } = null!;
}
