using Arpal.SiApi.WebApplication.Database;

namespace Arpal.SiApi.WebApplication.DTO
{
    public class SiapiServiziApikeyDTO
    {
        public decimal? IdServizio { get; set; }

        public decimal? IdApikey { get; set; }

        public DateTime? DataIni { get; set; }

        public DateTime? DataFin { get; set; }

        public decimal IdServizioApikey { get; set; }

        public SiapiApikeyDTO? IdApikeyNavigation { get; set; }

        public SiapiServiziDTO? IdServizioNavigation { get; set; }


        public static SiapiServiziApikeyDTO Create(SiapiServiziApikey obj, uint depthLevel)
        {
            var dto = new SiapiServiziApikeyDTO
            {
                DataFin = obj.DataFin,
                DataIni = obj.DataIni,
                IdApikey = obj.IdApikey,
                IdServizio = obj.IdServizio,
                IdServizioApikey = obj.IdServizioApikey     
            };

            var nextDepthLevel = depthLevel - 1;


            dto.IdServizioNavigation = depthLevel > 0 ? SiapiServiziDTO.Create(obj.IdServizioNavigation, nextDepthLevel) : null;

            return dto;
        }
    }
}
