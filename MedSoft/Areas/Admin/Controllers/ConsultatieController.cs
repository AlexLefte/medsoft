using Microsoft.AspNetCore.Mvc;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using MedSoft.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using MedSoft.Utility;
using System.Security.Claims;
using Stripe;

namespace MedSoft.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Rol_Admin)]
    public class ConsultatieController : Controller
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private IEnumerable<SelectListItem> _listaSpecializari;
        private IEnumerable<SelectListItem> _listaMedici;
        private IEnumerable<SelectListItem> _listaPacienti;
        private IEnumerable<SelectListItem> _listaMedicamente;
        #endregion

        #region Constructors
        public ConsultatieController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region Methods
        public IActionResult Index(string? status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            _listaSpecializari = _unitOfWork.Specializare.GetAll().
                Select(cat => new SelectListItem
                {
                    Text = cat.Nume,
                    Value = cat.SpecializareID.ToString()
                });

            List <ConsultatieVM> consultatieVMs = new List<ConsultatieVM>();

            IEnumerable<Consultatie> consultatii;
            if (status == null)
            {
                consultatii = _unitOfWork.Consultatie.GetAll(includeProperties: "Pacient,Medic,Medicamente");
            }
            else
            {
                consultatii = _unitOfWork.Consultatie.GetAll(c => c.Status == status, includeProperties: "Pacient,Medic,Medicamente");
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

            // Salveaza statusul in ViewData
            ViewData["Status"] = status;

            return View(consultatieVMs);
        }

        [HttpGet, ActionName("Update")]
        public IActionResult UpdateGET(int? id)
        {
            ConsultatieVM consultatieVM = GetConsultatieVMByID(id);

            return View(consultatieVM);
        }

        [HttpPost, ActionName("Update")]
        public IActionResult UpdatePOST(ConsultatieVM consultatieVM)
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

                // Actualizeaza o consultatie deja existenta
                _unitOfWork.DbContext.Call_SP_Modifica_Consultatie(consultatieVM.Consultatie.ConsultatieID, consultatieVM.Consultatie.Data,
                    consultatieVM.Consultatie.MedicID, consultatieVM.Consultatie.PacientID, consultatieVM.Consultatie.Diagnostic,
                    consultatieVM.Consultatie.MedicamentID, consultatieVM.Consultatie.Doza, consultatieVM.Consultatie.Status,
                    out errorMessage, out errorFlag);

                // Verifica flag-ul de erori
                if (errorFlag)
                {
                    TempData["error"] = "Consultatia nu a putut fi modificata.\n" +
                        errorMessage;
                }
                else
                {
                    _unitOfWork.Save();
                    TempData["success"] = "Consultatia a fost modificata cu succes!";
                }

                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Input invalid.";
                return RedirectToAction("Index");
            }
        }        

        [HttpGet, ActionName("Add")]
        public IActionResult AddGET(int? id)
        {
            ConsultatieVM consultatieVM = GetConsultatieVMByID(id);
            
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

        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> DeleteGET(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            ConsultatieVM? consultatieFromDb = GetConsultatieVMByID(id);

            if (consultatieFromDb == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            return View(consultatieFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(ConsultatieVM consultatieVM)
        {
            var toBeDeleted = _unitOfWork.Consultatie.Get(c => c.ConsultatieID == consultatieVM.Consultatie.ConsultatieID);
            if (toBeDeleted == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }

            try
            {
                _unitOfWork.Consultatie.Remove(toBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Consultatia a fost stearsa.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["error"] = "Consultatia nu a putut fi stearsa.";
                return RedirectToAction("Index");
            }           
        }
        #endregion

        #region Utils
        public ConsultatieVM GetConsultatieVMByID(int? id)
        {
            _listaMedici = GetMedicListItems();
            _listaPacienti = GetPacientListItems();
            _listaMedicamente = GetMedicamenteListItems();
            _listaSpecializari = GetSpecializareListItems();

            ConsultatieVM consultatieVM = new()
            {
                ListaMedici = _listaMedici,
                ListaPacienti = _listaPacienti,
                ListaMedicamente = _listaMedicamente,
                ListaSpecializari = _listaSpecializari.ToList(),
                Consultatie = new()
            };
            if (id != null && id != 0)
            {
                consultatieVM.Consultatie = _unitOfWork.Consultatie.Get(c => c.ConsultatieID == id, includeProperties: "Pacient,Medic,Medicamente");
                consultatieVM.Data = consultatieVM.Consultatie.Data;
                consultatieVM.Ora = consultatieVM.Data.ToString("HH:mm");
                consultatieVM.ListaSpecializari.First(s => s.Value == consultatieVM.Consultatie.Medic.SpecializareID.ToString()).Selected = true;
                consultatieVM.ListaIntervaleOrare = GetListaOreDisponibile(consultatieVM.Consultatie.Data, consultatieVM.Consultatie.MedicID, consultatieVM.Consultatie.ConsultatieID);
            }

            return consultatieVM;
        }


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

        public IEnumerable<SelectListItem> GetMedicListItems()
        {
            return _unitOfWork.Medic.GetAll().Select(
                       med => new SelectListItem
                       {
                           Text = med.NumeMedic + " " +
                               med.PrenumeMedic + " - " +
                               med.Specializare.Nume,
                           Value = med.MedicID.ToString()
                       });
        }

        public IEnumerable<SelectListItem> GetPacientListItems()
        {
            return _unitOfWork.Pacient.GetAll().Select(
                        p => new SelectListItem
                        {
                            Text = p.NumePacient + " " + p.PrenumePacient,
                            Value = p.PacientID.ToString()
                        });
        }

        public IEnumerable<SelectListItem> GetMedicamenteListItems()
        {
            return _unitOfWork.Medicamente.GetAll().Select(
                        m => new SelectListItem
                        {
                            Text = m.Denumire,
                            Value = m.MedicamentID.ToString()
                        });
        }

        public IEnumerable<SelectListItem> GetSpecializareListItems()
        {
            return _unitOfWork.Specializare.GetAll().
                Select(cat => new SelectListItem
                {
                    Text = cat.Nume,
                    Value = cat.SpecializareID.ToString(),
                });
        }
        #endregion
    }
}
