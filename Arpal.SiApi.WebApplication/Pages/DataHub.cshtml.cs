using Arpal.SiApi.WebApplication.Controllers;
using Arpal.SiApi.WebApplication.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace Arpal.SiApi.WebApplication.Pages
{

    public class DataHubModel : PageModel
    {
        protected IConfiguration _configuration;
        protected readonly ILogger<DataHubModel> _logger;

        public String BaseUrl { get; set; }
        public String AccordionServizio_BackgroundColor { get; set; }
        public String Label_DataHub_SubTitle { get; set; }
        public String Label_DataHub_Text_01 { get; set; }
        public String Label_DataHub_Text_02 { get; set; }
        public String Label_DataHub_Title { get; set; }

        public List<SiapiServizi> Servizi { get; set; }
        public List<SiapiLink> Links { get; set; }


        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }


        public DataHubModel(IConfiguration configuration, ILogger<DataHubModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected String ConnectionString
        {
            get
            {
                return _configuration?.GetConnectionString(Consts.ConnectionStringName) ?? "";
            }
        }

        public async Task OnGetAsync()
        {
            BaseUrl = _configuration.GetValue<string>(Consts.BaseUrl) ?? "https://aws.arpal.liguria.it/siapi/";
            AccordionServizio_BackgroundColor = _configuration.GetValue<string>(Consts.AccordionServizio_BackgroundColor) ?? "beige";
            Label_DataHub_SubTitle = _configuration.GetValue<string>(Consts.Label_DataHub_SubTitle) ?? "";
            Label_DataHub_Text_01 = _configuration.GetValue<string>(Consts.Label_DataHub_Text_01) ?? "";
            Label_DataHub_Text_02 = _configuration.GetValue<string>(Consts.Label_DataHub_Text_02) ?? "";
            Label_DataHub_Title = _configuration.GetValue<string>(Consts.Label_DataHub_Title) ?? "Titolo";

            using (var dbContext = new SiApiContext(_configuration))
            {
                string filtroVisibilitaServizi = (_configuration.GetValue<string>(Consts.FiltroVisibilitaServizi) ?? "").ToLowerInvariant();

                var today = DateTime.Now.Date;

                var serviziQuery = dbContext.SiapiServizis.Include(x => x.SiapiServiziParametris)
                                                          .Where(x => x.NomeServizio != null &&
                                                                      x.AuthRequired == "N" &&
                                                                      (x.DataIni == null || x.DataIni <= today) &&
                                                                      (x.DataFin == null || x.DataFin >= today));

                if (!string.IsNullOrWhiteSpace(filtroVisibilitaServizi))
                    serviziQuery = serviziQuery.Where(x => x.FiltroServizio != null && x.FiltroServizio.Contains(filtroVisibilitaServizi));

                Servizi = await serviziQuery.OrderBy(x => x.NomeServizio).ToListAsync();


                var LinksQuery = dbContext.SiapiLinks.Where(x => (x.DataIni == null || x.DataIni <= today) &&
                                                              (x.DataFin == null || x.DataFin >= today));

                if (!string.IsNullOrWhiteSpace(filtroVisibilitaServizi))
                    LinksQuery = LinksQuery.Where(x => x.FiltroLink != null && x.FiltroLink.Contains(filtroVisibilitaServizi));

                Links = await LinksQuery.OrderBy(x => x.NomeLink).ToListAsync();

                if (!string.IsNullOrEmpty(SearchString))
                {
                    var searchStringApp = SearchString.ToLower().Trim();
                    Servizi = Servizi.Where(s => s.NomeServizio.ToLower().Contains(searchStringApp)).ToList();
                    Links = Links.Where(s => s.NomeLink.ToLower().Contains(searchStringApp)).ToList();
                }
            }
        }
    }
}
