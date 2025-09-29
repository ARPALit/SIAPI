using Microsoft.IdentityModel.Tokens;

namespace Arpal.SiApi.WebApplication.JWT
{
    public class RefreshTokenContentDTO
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public RefreshTokenContentDTO(string refreshToken, DateTime refreshTokenExpiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
        }
    }
}
