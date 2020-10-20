using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using postersopmetaal.Models;
using postersopmetaal.Services;

namespace postersopmetaal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        List<Evenement> _evenementen = new List<Evenement>();
        private string apiUrl = Environment.GetEnvironmentVariable("AFAS_EVENEMENT_API");



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(EvenementSearchModel searchModel)
        {
            if (searchModel != null) apiUrl = EvenementBusinessLogic.AddFilter(apiUrl, searchModel);

            _evenementen = await AfasService.GetEvenementen(apiUrl);
           
            return View(_evenementen);
        }
        [HttpPost]
        public async Task<IActionResult> Index(List<long> CrId, bool isChecked = false)
        {      
            // Haal geselecteerde evenementen op
            _evenementen = await AfasService.GetEvenementenById(apiUrl, CrId);
            // Haal CdId op gebasseerd op BcCo en FilterMP
            var response = await AfasService.PostFactuur(_evenementen);
            // Geef response terug aan view
            ViewBag.response = response;

            return View(await AfasService.GetEvenementen(apiUrl));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
