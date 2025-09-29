using Arpal.SiApi.WebApplication.Database;
using Microsoft.EntityFrameworkCore;

namespace Arpal.SiApi.WebApplication.GraphQL.Schema
{
    public class Query
    {
        [GraphQLDeprecated("It was just a test.")]
        public string Instructions => "Demo demo";
    }
}
