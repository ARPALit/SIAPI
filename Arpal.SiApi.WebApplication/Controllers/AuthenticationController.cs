using Arpal.SiApi.Utils;
using Arpal.SiApi.WebApplication.Database;
using Arpal.SiApi.WebApplication.DTO;
using Arpal.SiApi.WebApplication.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Arpal.SiApi.WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : _BaseController
    {
        private const string ERROR_401 = "Contattare ARPAL indicando il proprio APIKEY per capire la motivazione del rifiuto al login.";

        public AuthenticationController(ILogger<AuthenticationController> logger, IConfiguration configuration) : base(logger, configuration) { }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<LoginResponseDTO>> LoginAsync([FromBody] LoginRequestDTO dto)
        {
            _logger.LogInformation($"Login trying this Username: {dto?.ApiKey}, IP: {Request.HttpContext.Connection.RemoteIpAddress?.ToString()}");

            if (String.IsNullOrWhiteSpace(dto?.ApiKey) || String.IsNullOrWhiteSpace(dto?.Password))
                return Unauthorized();

            using (var dbContext = new SiApiContext(_configuration))
            {
                var hash = HashManager.ComputeHash(dto.Password);
                var siApiKey = dbContext.SiapiApikeys.Include(x => x.SiapiServiziApikeys).ThenInclude(x => x.IdServizioNavigation)
                                                     .Where(x => x.Apikey == dto.ApiKey && x.Pwd == hash)
                                                     .FirstOrDefault();

                if (siApiKey == null)
                {
                    _logger.LogWarning($"ApiKey: {dto?.ApiKey}, non trovata o password errata");
                    return Unauthorized(ERROR_401);
                }

                // Date window validation
                var today = DateTime.Now.Date;
                if (today < siApiKey.DataIni || today > siApiKey.DataFin)
                {
                    _logger.LogWarning($"Login effettuato correttamente per questo ApiKey: {dto?.ApiKey}, ma l'apikey è scaduto {today:dd-MM-yyyy}, Inizio: {siApiKey.DataIni:dd-MM-yyyy}, Fine: {siApiKey.DataFin:dd-MM-yyyy}");
                    return Unauthorized(ERROR_401);
                }

                var apiKeyDTO = SiapiApikeyDTO.Create(siApiKey, 2);

                var tokenManager = new TokenManager(_configuration);
                var refreshTokenContent = tokenManager.GenerateRefreshToken();

                // Store refresh Token on DB
                siApiKey.RefreshToken = refreshTokenContent.RefreshToken;
                siApiKey.RefreshTokenExpiryTime = refreshTokenContent.RefreshTokenExpiryTime;
                dbContext.SaveChanges();

                return Ok(new LoginResponseDTO
                {
                    AccessToken = tokenManager.GenerateToken(apiKeyDTO),
                    RefreshToken = refreshTokenContent.RefreshToken
                });
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<ActionResult<LoginResponseDTO>> Refresh([FromBody] RefreshTokenRequestDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.AccessToken) || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest();

            _logger.LogInformation($"Refresh RefreshToken: {dto.RefreshToken}, AccessToken: {dto.AccessToken}");

            var tokenManager = new TokenManager(_configuration);

            var principal = tokenManager.RetrieveTokenContent(dto.AccessToken);

            using (var dbContext = new SiApiContext(_configuration))
            {
                var siApiKey = await dbContext.SiapiApikeys.Where(x => x.Apikey == principal.UserName &&
                                                                       x.RefreshToken == dto.RefreshToken)
                                                           .FirstOrDefaultAsync();
                if (siApiKey == null)
                    return BadRequest();

                if (siApiKey.RefreshTokenExpiryTime == null || DateTime.Now > siApiKey.RefreshTokenExpiryTime)
                    return Unauthorized("Token scaduto");

                var siApiKeyDTO = SiapiApikeyDTO.Create(siApiKey, 2);
                var newAccessToken = tokenManager.GenerateToken(siApiKeyDTO);
                var newRefreshToken = tokenManager.GenerateRefreshToken();

                siApiKey.RefreshToken = newRefreshToken.RefreshToken;
                siApiKey.RefreshTokenExpiryTime = newRefreshToken.RefreshTokenExpiryTime;
                dbContext.SaveChanges();

                return Ok(new LoginResponseDTO()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.RefreshToken
                });
            }
        }

        [HttpPost, Authorize]
        [Route("RefreshTokenRevoke")]
        public IActionResult Revoke()
        {
            var idApiKey = TokenManager.GetSid(Request);
            _logger.LogInformation($"Rovoke Refresh Token: idApiKey: {idApiKey}");

            using (var dbContext = new SiApiContext(_configuration))
            {
                var apiKey = dbContext.SiapiApikeys.Where(x => x.IdApikey == idApiKey).FirstOrDefault();
                if (apiKey == null)
                    return BadRequest();

                apiKey.RefreshToken = null;
                apiKey.RefreshTokenExpiryTime = null;

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest();

            var sId = TokenManager.GetSid(Request);

            _logger.LogInformation($"Change password, user id: {sId}");

            using (var dbContext = new SiApiContext(_configuration))
            {
                var siApiKey = await dbContext.SiapiApikeys.Where(x => x.IdApikey == sId)
                                                           .FirstOrDefaultAsync();

                if (siApiKey == null)
                    return BadRequest();

                var oldPasswordHash = HashManager.ComputeHash(dto.OldPassword);
                if (siApiKey.Pwd != oldPasswordHash)
                {
                    System.Threading.Thread.Sleep(3000);
                    return BadRequest("La password non è corretta");
                }

                var errorMessage = Utils.Password.IsPasswordValid(dto.NewPassword);
                if (!String.IsNullOrWhiteSpace(errorMessage))
                    return BadRequest(errorMessage);

                siApiKey.Pwd = HashManager.ComputeHash(dto.NewPassword);
                await dbContext.SaveChangesAsync();

                return Ok("Password cambiata");
            }
        }
    }
}
