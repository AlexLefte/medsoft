using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using MedSoft.Models.ViewModels;
using MedSoft.Utility;
using Stripe.Checkout;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace MedSoft.Areas.Pacient.Controllers
{
    [Area("Pacient")]
    [Authorize]
    public class ConsultatieController : Controller
    {
        private IUnitOfWork _unitOfWork;
        [BindProperty]
        public ConsultatieVM ConsultatieVM { get; set; }
        public ConsultatieController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IActionResult Index(string? status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<SelectListItem> _listaSpecializari = _unitOfWork.Specializare.GetAll().
                Select(cat => new SelectListItem
                {
                    Text = cat.Nume,
                    Value = cat.SpecializareID.ToString()
                });

            List<ConsultatieVM> consultatieVMs = new List<ConsultatieVM>();

            IEnumerable<Consultatie> consultatii;
            if (status == null)
            {
                consultatii = _unitOfWork.Consultatie.GetAll(
                    c => c.PacientID == userId, includeProperties: "Pacient,Medic,Medicamente");
            }
            else
            {
                consultatii = _unitOfWork.Consultatie.GetAll(
                    c => c.PacientID == userId && c.Status == status, includeProperties: "Pacient,Medic,Medicamente");
            }

            foreach (Consultatie consult in consultatii)
            {
                ConsultatieVM consultatieVM = new ConsultatieVM()
                {
                    Consultatie = consult,
                    ListaSpecializari = _listaSpecializari
                };
                consultatieVMs.Add(consultatieVM);
            }

            // Salveaza lista medicilor in ViewDataDictionary
            ViewData["ListaSpecializari"] = _listaSpecializari;

            return View(consultatieVMs);
        }

        [HttpGet, ActionName("Add")]
        public IActionResult AddGET(string? medicID)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ConsultatieVM consultatieVM = new()
            {
                ListaMedici = _unitOfWork.Medic.GetAll().
                    Select(med => new SelectListItem
                    {
                        Text = med.NumeMedic + " " +
                            med.PrenumeMedic + " - " +
                            med.Specializare.Nume,
                        Value = med.MedicID.ToString()
                    }),
                ListaSpecializari = _unitOfWork.Specializare.GetAll().
                    Select(cat => new SelectListItem
                    {
                        Text = cat.Nume,
                        Value = cat.SpecializareID.ToString(),
                    }),
                Consultatie = new()
                {
                    MedicID = medicID,
                    PacientID = userID,
                    Status = SD.ConsultStatusPending
                }
            };

            // Adauga medicul daca id-ul nu este null
            if (medicID != null)
            {
                consultatieVM.Consultatie.Medic = _unitOfWork.Medic.Get(m => m.MedicID == medicID);
            }

            return View(consultatieVM);
        }

        [HttpPost, ActionName("Add")]
        public IActionResult AddPOST(ConsultatieVM consultatieVM)
        {
            if (ModelState.IsValid)
            {
                // Creare TimeSpan
                var timespan = TimeSpan.Parse(consultatieVM.Ora);

                // Se concateneaza data din cele doua campuri de input
                consultatieVM.Consultatie.Data = new DateTime(
                    consultatieVM.Data.Year,
                    consultatieVM.Data.Month,
                    consultatieVM.Data.Day,
                    timespan.Hours,
                    timespan.Minutes,
                    timespan.Seconds);

                // Parametri de iesire
                string errorMessage;
                bool errorFlag;

                // Adauga o noua consultatie
                _unitOfWork.DbContext.Call_SP_Adauga_Consultatie(consultatieVM.Consultatie.Data, consultatieVM.Consultatie.MedicID,
                    consultatieVM.Consultatie.PacientID, consultatieVM.Consultatie.Diagnostic, consultatieVM.Consultatie.MedicamentID,
                    consultatieVM.Consultatie.Doza, consultatieVM.Consultatie.Status, out errorMessage, out errorFlag);

                // Verifica flag-ul de erori
                if (errorFlag)
                {
                    TempData["error"] = "Consultatia nu a putut fi adaugata.\n" +
                        errorMessage;
                }
                else
                {
                    _unitOfWork.Save();
                    TempData["success"] = "Consultatia a fost adaugata cu succes!";
                }

                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Input invalid.";
                return RedirectToAction("Index");
            }
        }

        #region Utils
        [HttpPost]
        public IActionResult GetMediciBySpecializare(int specializareID)
        {
            // Lista medicilor avand specializarea dorita
            IEnumerable<Models.Medic> medList = _unitOfWork.Medic.GetAll(
                med => med.SpecializareID == specializareID,
                includeProperties: "Specializare");

            // Convert the medici data to a format suitable for the AJAX response
            var mediciData = medList.Select(
                medic => new {
                    value = medic.MedicID,
                    text = "Dr. " +
                        medic.NumeMedic + " " +
                        medic.PrenumeMedic
                }).ToList();

            return Json(mediciData);
        }

        public IEnumerable<SelectListItem> GetListaOreDisponibile(DateTime data, string medicID, long consultatieID)
        {
            // Lista consultatiilor rezervate la data precizata
            IEnumerable<Consultatie> consultatiiList = _unitOfWork.Consultatie.GetAll(
                c => (c.Data.Date == data.Date && c.MedicID == medicID));

            // Alcatuieste lista orelor disponibile
            List<SelectListItem> oreDisponibile = new List<SelectListItem>();

            // Ora de inceput (08:00)
            DateTime oraCurenta = data.Date.AddHours(8);

            // Ora de final (21:00)
            DateTime oraFinala = data.Date.AddHours(21);

            // Itereaza prin orele disponibile cu intervale de 30 de minute
            while (oraCurenta <= oraFinala)
            {
                string oraFormata = oraCurenta.ToString("HH:mm");

                // Extrage consultatia inscrisa la aceeasi data
                Consultatie? consult = consultatiiList.FirstOrDefault(c => c.Data.ToString("HH:mm") == oraFormata);

                // Verifica daca ora curenta exista in lista de consultatii
                if (consult == null)
                {
                    oreDisponibile.Add(new SelectListItem
                    {
                        Text = oraFormata,
                        Value = oraFormata
                    });
                }
                else if (consult.ConsultatieID == consultatieID)
                {
                    oreDisponibile.Add(new SelectListItem
                    {
                        Text = oraFormata,
                        Value = oraFormata,
                        Selected = true
                    });
                }

                // Adauga 30 de minute
                oraCurenta = oraCurenta.AddMinutes(30);
            }

            return oreDisponibile;
        }

        [HttpPost]
        public IActionResult GetOreDisponibile(DateTime data, string medicID, long consultatieID)
        {
            // Lista consultatiilor rezervate la data precizata
            IEnumerable<Consultatie> consultatiiList = _unitOfWork.Consultatie.GetAll(
                c => (c.Data.Date == data.Date && c.MedicID == medicID));

            // Alcatuieste lista orelor disponibile
            List<string> oreDisponibile = new List<string>();

            // Ora de inceput (08:00)
            DateTime oraCurenta = data.Date.AddHours(8);

            // Ora de final (21:00)
            DateTime oraFinala = data.Date.AddHours(21);

            // Itereaza prin orele disponibile cu intervale de 30 de minute
            while (oraCurenta <= oraFinala)
            {
                string oraFormata = oraCurenta.ToString("HH:mm");

                // Extrage consultatia inscrisa la aceeasi data
                Consultatie? consult = consultatiiList.FirstOrDefault(c => c.Data.ToString("HH:mm") == oraFormata);

                // Verifica daca ora curenta exista in lista de consultatii
                if (consult == null || consult.ConsultatieID == consultatieID)
                {
                    oreDisponibile.Add(oraFormata);
                }

                // Adauga 30 de minute
                oraCurenta = oraCurenta.AddMinutes(30);
            }

            var response = new
            {
                OreDisponibile = oreDisponibile,
                // OraSelectata = oraSelectata
            };

            // return Json(response);
            return Json(oreDisponibile);
        }
        #endregion
    }
}
