using Arpal.SiApi.WebApplication.Database;

namespace Arpal.SiApi.WebApplication.DTO
{
    public class SiapiServiziDTO
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

        public static SiapiServiziDTO? Create(SiapiServizi? obj, uint depthLevel)
        {
            if (obj == null)
                return null;

            var dto = new SiapiServiziDTO()
            {
                AuthRequired = obj.AuthRequired,
                ApikeyRequired = obj.ApikeyRequired,
                DataFin = obj.DataFin,
                DataIni = obj.DataIni,
                DescServizio = obj.DescServizio,
                HelpServizio = obj.HelpServizio,
                NomeServizio = obj.NomeServizio,
                SqlStatement = obj.SqlStatement,
                IdServizio = obj.IdServizio,
                FiltroServizio = obj.FiltroServizio

            };

            return dto;
        }
    }
}
