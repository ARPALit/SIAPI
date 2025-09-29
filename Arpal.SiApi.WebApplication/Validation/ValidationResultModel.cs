using log4net;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Arpal.SiApi.WebApplication.Validation
{
    public class ValidationResultModel
    {
        private static ILog _Logger = LogManager.GetLogger(typeof(LicenseManager));

        public string Message { get; }
        public List<ValidationError> Errors { get; }


        public ValidationResultModel(ActionContext context)
        {
            Message = "Validation Failed";

            var request = context.HttpContext.Request;
            var url = $"{request.Method} {request.Scheme}://{request.Host.Value}{request.Path}{request.QueryString}";

            try
            {
                var errors = context.ModelState.Keys.SelectMany(key => context.ModelState[key].Errors.Select(x => new ValidationError(key, 0, x.ErrorMessage)))
                                                    .ToList();
                var errorsArray = errors.Select(x => x.ToString()).ToArray();
                if (errorsArray?.Length > 0)
                    _Logger.Warn($"Api: {url}, with this errors: {String.Join(", ", errorsArray)}");
            }
            catch (Exception ex)
            {
                _Logger.Warn($"Api: {url}, with errors, but we were unable to read errors", ex);
            }

            Errors = new List<ValidationError>() { new ValidationError("No Message", 0, "Non possiamo darvi informazioni più dettagliate, contattare ARPAL grazie.") };
        }
    }
}
