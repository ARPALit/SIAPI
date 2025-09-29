using Arpal.SiApi.WebApplication.DTO;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Arpal.SiApi.WebApplication.JWT
{
    public class TokenManager
    {
        private readonly IConfiguration _configuration;

        public const String SecurityKey = "AGga-ep6UuxhD2F6TazGr+8nMczV5-GGGyT?Eu*fXvZC7Dtre";

        public const int DefaultExpireTimeInMinutes = 120;
        public const int DefaultRefreshTokenExpireTimeInMinutes = 600;


        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public String GenerateToken(SiapiApikeyDTO siApiKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (_configuration == null)
                throw new ConfigurationErrorsException("Token manager with no configuration");

            var expireTimeInt = DefaultExpireTimeInMinutes;
            string expireTime = _configuration[Consts.Jwt_ExpireTimeInMinutesName];
            if (!int.TryParse(expireTime, out expireTimeInt))
                expireTimeInt = DefaultExpireTimeInMinutes;

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.UniqueName, siApiKey.Apikey),
                new Claim(JwtRegisteredClaimNames.Jti, $"{siApiKey.IdApikey}"),
                new Claim(JwtRegisteredClaimNames.Sid, $"{siApiKey.IdApikey}"),
                new Claim(JwtRegisteredClaimNames.Sub, siApiKey.Apikey),
            };

            if (siApiKey.SiapiServiziApikeys != null && siApiKey.SiapiServiziApikeys.Count() > 0)
            {
                var today = DateTime.Now.Date;
                var lista = siApiKey.SiapiServiziApikeys.Where(x => today >= x.DataIni && today <= x.DataFin &&
                                                                    today >= x.IdServizioNavigation?.DataIni && today <= x.IdServizioNavigation?.DataFin)
                                                        .Select(x => x.IdServizioApikey).ToList();

                claims.Add(new Claim(Consts.Jwt_Claim_Services, string.Join(",", lista)));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration[Consts.Jwt_AudienceName],
                Issuer = _configuration[Consts.Jwt_IssuerName],
                Expires = DateTime.Now.AddMinutes(expireTimeInt),
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha384Signature),
                Subject = new ClaimsIdentity(claims)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenInString = tokenHandler.WriteToken(token);

            return tokenInString;
        }

        public RefreshTokenContentDTO GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                var expireTimeInt = DefaultRefreshTokenExpireTimeInMinutes;
                string expireTime = _configuration[Consts.Jwt_RefreshTokenExpireTimeInMinutesName];
                if (!int.TryParse(expireTime, out expireTimeInt))
                    expireTimeInt = DefaultRefreshTokenExpireTimeInMinutes;

                return new RefreshTokenContentDTO(Convert.ToBase64String(randomNumber), DateTime.Now.AddMinutes(expireTimeInt));
            }
        }

        public TokenContentDTO RetrieveTokenContent(String token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (_configuration == null)
                throw new ConfigurationErrorsException("Token manager with no configuration");

            var securtyToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (securtyToken?.Claims == null)
                return new TokenContentDTO();

            return new TokenContentDTO
            {
                SecurityToken = securtyToken,
                Sid = securtyToken.Claims.Where(x => x.Type != null && x.Type.Equals(JwtRegisteredClaimNames.Sid)).FirstOrDefault().Value,
                Token = token,
                UserName = securtyToken.Claims.Where(x => x.Type.Equals(JwtRegisteredClaimNames.UniqueName)).FirstOrDefault().Value
            };
        }

        public static int GetSid(HttpRequest request)
        {
            try
            {
                return Convert.ToInt32(request.HttpContext.User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sid).FirstOrDefault()?.Value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
