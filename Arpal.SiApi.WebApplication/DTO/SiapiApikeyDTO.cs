using Arpal.SiApi.WebApplication.Database;

namespace Arpal.SiApi.WebApplication.DTO
{
    public class SiapiApikeyDTO
    {
        public decimal IdApikey { get; set; }
        public String? Apikey { get; set; }
        public DateTime? DataIni { get; set; }
        public DateTime? DataFin { get; set; }
        public String? DescApikey { get; set; }
        public String? Pwd { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public virtual List<SiapiServiziApikeyDTO> SiapiServiziApikeys { get; set; } = new List<SiapiServiziApikeyDTO>();


        public static SiapiApikeyDTO Create(SiapiApikey obj, uint depthLevel)
        {
            var dto = new SiapiApikeyDTO
            {
                IdApikey = obj.IdApikey,
                Apikey = obj.Apikey,
                DataFin = obj.DataFin,
                DataIni = obj.DataIni,
                DescApikey = obj.DescApikey,
                RefreshToken = obj.RefreshToken,
                RefreshTokenExpiryTime = obj.RefreshTokenExpiryTime
            };

            var nextDepthLevel = depthLevel - 1;

            dto.SiapiServiziApikeys = depthLevel > 0 && obj.SiapiServiziApikeys != null
                                   ? obj.SiapiServiziApikeys.Select(x => SiapiServiziApikeyDTO.Create(x, nextDepthLevel)).ToList()
                                   : null;

            return dto;
        }
    }
}
