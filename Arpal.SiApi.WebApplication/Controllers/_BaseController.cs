using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace Arpal.SiApi.WebApplication.Controllers
{
    public class _BaseController : ControllerBase
    {
        protected IConfiguration _configuration;
        protected readonly ILogger<_BaseController> _logger;


        public _BaseController(ILogger<_BaseController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        
        protected String ConnectionString
        {
            get
            {
                return _configuration?.GetConnectionString(Consts.ConnectionStringName) ?? "";
            }
        }
    }
}
