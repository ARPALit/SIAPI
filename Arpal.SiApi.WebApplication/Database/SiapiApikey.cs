using System;
using System.Collections.Generic;

namespace Arpal.SiApi.WebApplication.Database;

public partial class SiapiApikey
{
    public decimal IdApikey { get; set; }

    public string? Apikey { get; set; }

    public DateTime? DataIni { get; set; }

    public DateTime? DataFin { get; set; }

    public string? DescApikey { get; set; }

    public string? Pwd { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<SiapiQuerylog> SiapiQuerylogs { get; set; } = new List<SiapiQuerylog>();

    public virtual ICollection<SiapiServiziApikey> SiapiServiziApikeys { get; set; } = new List<SiapiServiziApikey>();
}
