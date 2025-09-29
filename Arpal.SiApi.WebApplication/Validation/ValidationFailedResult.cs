using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arpal.SiApi.WebApplication.Validation
{
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ActionContext context) : base(new ValidationResultModel(context))
        {
            // By Default is 0
            StatusCode = StatusCodes.Status422UnprocessableEntity;
        }
    }
}
