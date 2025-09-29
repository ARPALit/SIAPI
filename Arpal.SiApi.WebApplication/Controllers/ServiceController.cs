using Arpal.SiApi.WebApplication.Database;
using Arpal.SiApi.WebApplication.DTO;
using Arpal.SiApi.WebApplication.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Arpal.SiApi.WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : _BaseController
    {
        public ServiceController(ILogger<ServiceController> logger, IConfiguration configuration) : base(logger, configuration) { }

        [HttpGet]
        [Route("NoAuthenticationService")]
        public async Task<ActionResult> NoAuthenticationService()
        {
            _logger.LogInformation($"User Services, APiKey: {Request.HttpContext.User.Identity?.Name}");

            string filtroVisibilitaServizi = (_configuration.GetValue<string>(Consts.FiltroVisibilitaServizi) ?? "").ToLowerInvariant();

            using (var dbContext = new SiApiContext(_configuration))
            {
                var today = DateTime.Now.Date;

                var serviziQuery = dbContext.SiapiServizis.Include(x => x.SiapiServiziParametris)
                                                          .Where(x => x.NomeServizio != null &&
                                                                      x.AuthRequired == "N" &&
                                                                      (x.DataIni == null || x.DataIni <= today) &&
                                                                      (x.DataFin == null || x.DataFin >= today));

                if (!string.IsNullOrWhiteSpace(filtroVisibilitaServizi))
                    serviziQuery = serviziQuery.Where(x => x.FiltroServizio != null && x.FiltroServizio.Contains(filtroVisibilitaServizi));


                var servizi = await serviziQuery.ToListAsync();

                var obj = new
                {
                    servizi = servizi.Select(x => new
                    {
                        apiKeyRichiesto = x.ApikeyRequired == "Y" ? "SI" : "NO",
                        autenticazioneRichiesta = x.AuthRequired == "Y" ? "SI" : "NO",
                        help = x.HelpServizio ?? "",
                        nome = x.NomeServizio,
                        parametri = x.SiapiServiziParametris != null
                                      ? x.SiapiServiziParametris.Select(y => new
                                      {
                                          help = y.HelpParametro ?? "",
                                          nome = y.UserField,
                                          tipo = y.Datatype
                                      }).ToList()
                                      : []
                    })
                };

                return Ok(obj);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("UserService")]
        public async Task<ActionResult> UserServices()
        {
            _logger.LogInformation($"User Services, APiKey: {Request.HttpContext.User.Identity?.Name}");

            var idApiKey = TokenManager.GetSid(Request);

            string filtroVisibilitaServizi = (_configuration.GetValue<string>(Consts.FiltroVisibilitaServizi) ?? "").ToLowerInvariant();

            using (var dbContext = new SiApiContext(_configuration))
            {
                var today = DateTime.Now.Date;
                var serviziNavigationQuery = dbContext.SiapiServiziApikeys.Include(x => x.IdServizioNavigation).ThenInclude(x => x.SiapiServiziParametris)
                                                                           .Include(x => x.IdApikeyNavigation)
                                                                           .Where(x => x.IdServizioNavigation != null &&
                                                                                       x.IdApikey == idApiKey &&
                                                                                       (x.DataIni == null || x.DataIni <= today) &&
                                                                                       (x.DataFin == null || x.DataFin >= today) &&
                                                                                       (x.IdServizioNavigation.DataIni == null || x.IdServizioNavigation.DataIni <= today) &&
                                                                                       (x.IdServizioNavigation.DataFin == null || x.IdServizioNavigation.DataFin >= today))
                                                                           .Select(x => x.IdServizioNavigation);

                if (!string.IsNullOrWhiteSpace(filtroVisibilitaServizi))
                    serviziNavigationQuery = serviziNavigationQuery.Where(x => x.FiltroServizio != null && x.FiltroServizio.Contains(filtroVisibilitaServizi));

                var serviziNavigation = await serviziNavigationQuery.ToListAsync();


                // Prendo i servizi che non sono dell'utente ma che sono pubblici
                var servizi = await dbContext.SiapiServizis.Include(x => x.SiapiServiziParametris)
                                                           .Where(x => x.AuthRequired == "N").ToListAsync();

                foreach (var item in servizi)
                {
                    if (!serviziNavigation.Any(x => x.NomeServizio == item.NomeServizio))
                        serviziNavigation.Add(item);
                }

                var obj = new
                {
                    servizi = serviziNavigation.Select(x => new
                    {
                        apiKeyRichiesto = x.ApikeyRequired == "Y" ? "SI" : "NO",
                        autenticazioneRichiesta = x.AuthRequired == "Y" ? "SI" : "NO",
                        help = x.HelpServizio ?? "",
                        nome = x.NomeServizio,
                        poarametri = x.SiapiServiziParametris != null
                                                    ? x.SiapiServiziParametris.Select(y => new
                                                    {
                                                        help = y.HelpParametro ?? "",
                                                        nome = y.UserField,
                                                        tipo = y.Datatype
                                                    }).ToList()
                                                    : []
                    })
                };

                return Ok(obj);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("Query/{nomeServizio}")]
        public async Task<ActionResult> ServiceQueryPost([FromRoute] string nomeServizio, [FromBody] DynamicQueryRequestDTO? dto)
        {
            _logger.LogInformation($"Query POST, APiKey: {Request.HttpContext.User.Identity?.Name} - Service Name: {nomeServizio}");

            string filtroVisibilitaServizi = (_configuration.GetValue<string>(Consts.FiltroVisibilitaServizi) ?? "").ToLowerInvariant();

            var idApiKey = TokenManager.GetSid(Request);
            SiapiServiziApikey? servizioApiKey = null;
            using (var dbContext = new SiApiContext(_configuration))
            {
                nomeServizio = nomeServizio.ToLower();

                servizioApiKey = await dbContext.SiapiServiziApikeys.Include(x => x.IdServizioNavigation).ThenInclude(x => x.SiapiServiziParametris)
                                                                    .Include(x => x.IdApikeyNavigation)
                                                                    .Where(x => x.IdServizioNavigation != null && x.IdServizioNavigation.NomeServizio != null &&
                                                                                x.IdServizioNavigation.NomeServizio.ToLower() == nomeServizio &&
                                                                                x.IdApikey == idApiKey)
                                                                    .FirstOrDefaultAsync();

                // 1. api validation
                if (servizioApiKey?.IdServizioNavigation == null)
                {
                    _logger.LogWarning($"Query, APiKey: {Request.HttpContext.User.Identity?.Name} - Service Name: {nomeServizio} NOT FOUND ");
                    return BadRequest($"Servizio inesistente o non configurato per la ApiKey utilizzata");
                }

                // 2. Servizi interni
                if (!string.IsNullOrWhiteSpace(filtroVisibilitaServizi) && (
                    servizioApiKey.IdServizioNavigation.FiltroServizio == null || !servizioApiKey.IdServizioNavigation.FiltroServizio.Contains(filtroVisibilitaServizi)))
                {
                    _logger.LogWarning($"Query, APiKey: {Request.HttpContext.User.Identity?.Name} - Service Name: {nomeServizio} Called from Outside ");
                    return BadRequest($"Servizio inesistente o non disponibile per questa installazione");
                }

                // 3. Date window validation
                var today = DateTime.Now.Date;
                if (servizioApiKey.DataIni == null || today < servizioApiKey.DataIni || today > servizioApiKey.DataFin ||
                    servizioApiKey.IdServizioNavigation.DataIni == null || today < servizioApiKey.IdServizioNavigation.DataIni || today > servizioApiKey.IdServizioNavigation.DataFin)
                {
                    _logger.LogWarning($"Il servizio {nomeServizio} è scaduto {today:dd-MM-yyyy}, Inizio: {servizioApiKey.IdServizioNavigation.DataIni:dd-MM-yyyy}, Fine: {servizioApiKey.IdServizioNavigation.DataFin:dd-MM-yyyy}");
                    return BadRequest("Servizio Scaduto");
                }

                // 4. Logging if query is good
                await dbContext.SiapiQuerylogs.AddAsync(new SiapiQuerylog
                {
                    IdApikey = idApiKey,
                    IdServizio = servizioApiKey.IdServizioNavigation.IdServizio,
                    Json = dto != null ? JsonSerializer.Serialize(dto) : "",
                    QueryDate = DateTime.Now
                });

                await dbContext.SaveChangesAsync();
            }

            // 4. Execute query
            return await ExecuteQuery(servizioApiKey.IdServizioNavigation, dto?.Parametri);
        }

        [HttpGet]
        [Route("Query/{nomeServizio}")]
        public async Task<ActionResult> ServiceQueryGet([FromRoute] string nomeServizio)
        {
            var apiKeyObj = Request.Query.Select(x => new { x.Key, x.Value })
                                         .Where(x => x.Key.ToUpper() == "APIKEY")
                                         .FirstOrDefault();

            _logger.LogInformation($"Query GET, ServiceName: {nomeServizio}, ApiKey: {apiKeyObj?.Value ?? "NO API"}");

            string filtroVisibilitaServizi = (_configuration.GetValue<string>(Consts.FiltroVisibilitaServizi) ?? "").ToLowerInvariant();

            // 1. Validate service name
            if (string.IsNullOrWhiteSpace(nomeServizio))
                return BadRequest("Nome servizio errato");

            var today = DateTime.Now.Date;

            using (var dbContext = new SiApiContext(_configuration))
            {
                nomeServizio = nomeServizio.ToLower();

                var servizio = await dbContext.SiapiServizis.Include(x => x.SiapiServiziParametris)
                                                            .Where(x => x.NomeServizio != null && x.NomeServizio.ToLower() == nomeServizio)
                                                            .FirstOrDefaultAsync();
                if (servizio == null)
                    return BadRequest("Nome servizio errato");

                if (servizio.AuthRequired?.ToUpper() != "N")
                    return Unauthorized("Il servizio richiede autenticazione per essere utilizzato");

                // 2. Servizi interni
                if (!string.IsNullOrWhiteSpace(filtroVisibilitaServizi) && (
                   servizio.FiltroServizio == null || !servizio.FiltroServizio.Contains(filtroVisibilitaServizi)))
                {
                    _logger.LogWarning($"Query, APiKey: {Request.HttpContext.User.Identity?.Name} - Service Name: {nomeServizio} Called from Outside ");
                    return BadRequest($"Servizio inesistente o non disponibile per questa installazione");
                }

                // 3. Validate apikey
                if (servizio.ApikeyRequired.ToUpper() == "Y")
                {
                    if (apiKeyObj?.Key == null)
                        return BadRequest("Questo servizio richiede apiKey tra i parametri e.g. http:....?apiKey=myApiKey");

                    var siString = apiKeyObj?.Value.ToString();
                    var siApiKey = dbContext.SiapiApikeys.Where(x => x.Apikey == siString).FirstOrDefault();

                    if (siApiKey == null)
                        return BadRequest("Questo servizio richiede apiKey tra i parametri e.g. http:....?apiKey=myApiKey");

                    var servizioApiKey = await dbContext.SiapiServiziApikeys.Where(x => x.IdServizio == servizio.IdServizio &&
                                                                                        x.IdApikey == siApiKey.IdApikey)
                                                                            .FirstOrDefaultAsync();
                    if (servizioApiKey == null)
                        return BadRequest("Questo servizio richiede apiKey tra i parametri e.g. http:....?apiKey=myApiKey");

                    // Date validation
                    if (servizioApiKey.DataIni == null || today < servizioApiKey.DataIni || today > servizioApiKey.DataFin)
                    {
                        _logger.LogWarning($"Il servizio {nomeServizio} è scaduto {today:dd-MM-yyyy}, Inizio: {servizioApiKey.DataIni:dd-MM-yyyy}, Fine: {servizioApiKey.DataFin:dd-MM-yyyy}");
                        return BadRequest("Servizio Scaduto");
                    }
                }

                // 4. Date validation
                if (servizio.DataIni == null || today < servizio.DataIni || today > servizio.DataFin)
                {
                    _logger.LogWarning($"Il servizio {nomeServizio} è scaduto {today:dd-MM-yyyy}, Inizio: {servizio.DataIni:dd-MM-yyyy}, Fine: {servizio.DataFin:dd-MM-yyyy}");
                    return BadRequest("Servizio Scaduto");
                }


                // 5.Prepare paramenters
                var parameters = Request.Query.ToList().Where(x => x.Key.ToUpper() != "APIKEY")
                                                       .Select(x => new DynamicQueryParameterDTO { Alias = x.Key, Value = x.Value })
                                                       .ToList();

                // 6. Logging if query is good
                //await dbContext.SiapiQuerylogs.AddAsync(new SiapiQuerylog
                //{
                //    IdApikey = idApiKey,
                //    IdServizio = servizio.IdServizio,
                //    Json = JsonSerializer.Serialize(Request.QueryString.ToString()),
                //    QueryDate = DateTime.Now
                //});

                await dbContext.SaveChangesAsync();

                // 6. Execute query
                return await ExecuteQuery(servizio, parameters);
            }
        }

        private async Task<ActionResult> ExecuteQuery(SiapiServizi idServizioNavigation, List<DynamicQueryParameterDTO>? parametri)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                connection.Open();

                // Query
                var query = idServizioNavigation.SqlStatement;

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    // Parameters
                    if (idServizioNavigation.SiapiServiziParametris.Count > 0)
                    {
                        foreach (var siApiParameter in idServizioNavigation.SiapiServiziParametris)
                        {
                            // Veriica se il parametro è mandatorio e in caso mettere NULL se non c'è
                            var dtoParamenter = parametri?.Where(x => x.Alias != null &&
                                                                      x.Alias?.ToUpper() == siApiParameter.UserField?.ToUpper())
                                                            .FirstOrDefault();

                            if (dtoParamenter == null && siApiParameter.Mandatory == "Y")
                            {
                                return BadRequest($"Attenzione il parametro {siApiParameter.UserField} è obbligatorio");
                            }
                            else
                            {
                                if (dtoParamenter?.Value != null)
                                {
                                    //  Date format ISO 8601 combined date and time format.
                                    if (siApiParameter.Datatype == "DATA")
                                    {
                                        OracleParameter dateParam = new OracleParameter(siApiParameter?.FieldAlias, OracleDbType.Date);
                                        DateTime val;
                                        if (DateTime.TryParse(dtoParamenter.Value, out val))
                                            dateParam.Value = val;
                                        else
                                            return BadRequest($"Attenzione il parametro {siApiParameter?.UserField} non è una data in formato 'ISO 8601 combined date and time format': 2024-10-20T15:30:10");
                                        command.Parameters.Add(dateParam);
                                    }
                                    else if (siApiParameter.Datatype == "DECIMAL")
                                    {
                                        OracleParameter dateParam = new OracleParameter(siApiParameter?.FieldAlias, OracleDbType.Decimal);

                                        decimal val;
                                        if (decimal.TryParse(dtoParamenter.Value, out val))
                                            dateParam.Value = val;
                                        else
                                            return BadRequest($"Attenzione il parametro {siApiParameter?.UserField} non è un numero decimale");

                                        command.Parameters.Add(dateParam).Value = val;
                                    }
                                    else if (siApiParameter.Datatype == "INTEGER")
                                    {
                                        OracleParameter intParam = new OracleParameter(siApiParameter?.FieldAlias, OracleDbType.Int64);

                                        long val;
                                        if (Int64.TryParse(dtoParamenter.Value, out val))
                                            intParam.Value = val;
                                        else
                                            return BadRequest($"Attenzione il parametro {siApiParameter?.UserField} non è un numero intero");

                                        command.Parameters.Add(intParam).Value = val;
                                    }
                                    else if (siApiParameter.Datatype == "TEXT")
                                    {
                                        command.Parameters.Add(new OracleParameter($"{siApiParameter?.FieldAlias}", OracleDbType.Varchar2)).Value = dtoParamenter.Value;
                                        //command.Parameters.Add($"@{siApiParameter?.FieldAlias}", dtoParamenter.Value).Value = dtoParamenter.Value;
                                    }
                                    else
                                    {
                                        // This is a configuration error
                                        _logger.LogError($"SiapiServizio ID: {idServizioNavigation.IdServizio} - Parametro ID: {siApiParameter.IdServizioParametro} con Tipo {siApiParameter.Datatype} ERRATO");
                                        return new StatusCodeResult(500);
                                    }
                                }
                                else
                                    command.Parameters.Add($"{siApiParameter?.FieldAlias}", null).Value = null;
                            }
                        }
                    }

                    // ******************************************************************************************
                    // Mandatory to support multiple parameters
                    // ******************************************************************************************
                    command.BindByName = true;

                    using (OracleDataReader reader = (OracleDataReader)(await command.ExecuteReaderAsync()))
                    {
                        var list = new List<dynamic>();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                dynamic dynamicData = new ExpandoObject();
                                var dynamicDictionary = (IDictionary<string, object>)dynamicData;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    if (reader[i].GetType().Name == "DBNull")
                                    {
                                        dynamicDictionary[columnName] = null;
                                    }
                                    else
                                    {
                                        dynamic value = reader[i];
                                        dynamicDictionary[columnName] = value;
                                    }
                                }
                                list.Add(dynamicData);
                            }
                        }

                        // Return result in Json
                        return Ok(list);
                    }
                }
            }
        }
    }
}

