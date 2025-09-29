using Microsoft.EntityFrameworkCore;

namespace Arpal.SiApi.WebApplication.Database
{
    public partial class SiApiContext : DbContext
    {
        public IConfiguration _configuration { get; set; }
        public String _connectionString { get; set; }


        public SiApiContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SiApiContext(String connectionString)
        {
            _connectionString = connectionString;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionsString = _configuration?.GetConnectionString(Consts.ConnectionStringName);
            if (String.IsNullOrEmpty(connectionsString))
                optionsBuilder.UseOracle(_connectionString);
            else
                optionsBuilder.UseOracle(connectionsString);
        }
    }
}
