using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using MedSoft.Utility;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using MedSoft.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace MedSoft.Areas.Pacient.Controllers
{
    [Area("Pacient")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string? specializare)
        {
            IEnumerable<SelectListItem> listaSpecializari = _unitOfWork.Specializare.GetAll().
                    Select(cat => new SelectListItem
                    {
                        Text = cat.Nume,
                        Value = cat.SpecializareID.ToString(),
                        Selected = specializare == cat.SpecializareID.ToString()
                    });

            IEnumerable<Models.Medic> listaMedici;
            if (specializare == null)
            {
                listaMedici = _unitOfWork.Medic.GetAll(includeProperties: "Specializare");
            }
            else
            {
                listaMedici = _unitOfWork.Medic.GetAll(m => m.SpecializareID == long.Parse(specializare), includeProperties: "Specializare");
            }

            // Salveaza lista medicilor in ViewDataDictionary
            ViewData["ListaSpecializari"] = listaSpecializari;

            return View(listaMedici);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}