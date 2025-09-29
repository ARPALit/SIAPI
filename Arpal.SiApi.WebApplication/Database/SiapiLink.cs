using System;
using System.Collections.Generic;

namespace Arpal.SiApi.WebApplication.Database;

public partial class SiapiLink
{
    public decimal IdLink { get; set; }

    public string? DescLink { get; set; }

    public string? Link { get; set; }

    public DateTime? DataIni { get; set; }

    public DateTime? DataFin { get; set; }

    public string NomeLink { get; set; } = null!;

    public string? HelpLink { get; set; }

    public string? LinkInterno { get; set; }

    public string FiltroLink { get; set; } = null!;
}
