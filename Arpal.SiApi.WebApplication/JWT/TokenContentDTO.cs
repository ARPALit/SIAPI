using Microsoft.IdentityModel.Tokens;

namespace Arpal.SiApi.WebApplication.JWT
{
    public class TokenContentDTO
    {
        public SecurityToken? SecurityToken { get; set; }
        public string? Sid { get; set; }
        public string? Token { get; set; }
        public string? UserName { get; set; }
    }
}
