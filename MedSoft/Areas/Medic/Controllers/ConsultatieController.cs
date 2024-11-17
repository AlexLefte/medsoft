using Microsoft.AspNetCore.Mvc;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using MedSoft.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using MedSoft.Utility;
using System.Security.Claims;

namespace MedSoft.Areas.Medic.Controllers
{
    [Area("Medic")]
    [Authorize(Roles = SD.Rol_Medic)]
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
                consultatii = _unitOfWork.Consultatie.GetAll(c => c.MedicID == userId,
                    includeProperties: "Pacient,Medic,Medicamente");
            }
            else
            {
                consultatii = _unitOfWork.Consultatie.GetAll(c => c.Status == status && c.MedicID == userId, 
                    includeProperties: "Pacient,Medic,Medicamente");
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

        [HttpGet, ActionName("ChangeStatus")]
        public IActionResult ChangeStatus(int? id, string? status)
        {
            if (id == null || id.Value == 0 || status == null || status == string.Empty)
            {
                TempData["error"] = "Consultatia nu a putut fi identificata";
                return RedirectToAction("Index");
            }
            else
            {
                // Interogheaza consultatia avand respectivul id
                Consultatie consultatie = _unitOfWork.Consultatie.Get(c => c.ConsultatieID == id,
                    includeProperties: "Medic,Pacient,Medicamente");

                // Verifica daca a fost gasita o consultatie cu acest id
                if (consultatie == null)
                {
                    TempData["error"] = "Consultatia nu a putut fi identificata";
                    return RedirectToAction("Index");
                }

                // Actualizeaza statusul
                consultatie.Status = status;

                // Actualizeaza consultatia
                // Parametri de iesire
                string errorMessage;
                bool errorFlag;

                // Actualizeaza o consultatie deja existenta
                _unitOfWork.DbContext.Call_SP_Modifica_Consultatie(consultatie.ConsultatieID, consultatie.Data,
                    consultatie.MedicID, consultatie.PacientID, consultatie.Diagnostic,
                    consultatie.MedicamentID, consultatie.Doza, consultatie.Status,
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
        }

        [HttpGet, ActionName("Complete")]
        public IActionResult CompleteGET(int? id)
        {
            if (id == null || id.Value == 0)
            {
                TempData["error"] = "Consultatia nu a putut fi identificata";
                return RedirectToAction("Index");
            }
            else
            {
                // Interogheaza consultatia avand respectivul id
                Consultatie consultatie = _unitOfWork.Consultatie.Get(c => c.ConsultatieID == id,
                    includeProperties: "Medic,Pacient,Medicamente");
              
                // Verifica daca a fost gasita o consultatie cu acest id
                if (consultatie == null)
                {
                    TempData["error"] = "Consultatia nu a putut fi identificata";
                    return RedirectToAction("Index");
                }

                // Creaza o instanta a view model Finalizeaza Consultatie
                FinalizeazaConsultatieVM fc = new()
                {
                    Consultatie = consultatie
                };

                // Adauga lista de medicamente
                var listaMedicamente = _unitOfWork.Medicamente.GetAll().Select(
                        m => new SelectListItem
                        {
                            Text = m.Denumire,
                            Value = m.MedicamentID.ToString()
                        });
                ViewData["ListaMedicamente"] = listaMedicamente;

                return View(fc);
            }
        }

        [HttpPost, ActionName("Complete")]
        public IActionResult CompletePOST(FinalizeazaConsultatieVM consultatieVM)
        {
            if (ModelState.IsValid)
            { 
                // Parametri de iesire
                string errorMessage;
                bool errorFlag;

                // Actualizeaza o consultatie deja existenta
                _unitOfWork.DbContext.Call_SP_Modifica_Consultatie(consultatieVM.Consultatie.ConsultatieID, consultatieVM.Consultatie.Data,
                    consultatieVM.Consultatie.MedicID, consultatieVM.Consultatie.PacientID, consultatieVM.Diagnostic,
                    consultatieVM.MedicamentID, consultatieVM.Doza, SD.ConsultStatusCompleted,
                    out errorMessage, out errorFlag);

                // Verifica flag-ul de erori
                if (errorFlag)
                {
                    TempData["error"] = "Consultatia nu a putut fi finalizata.\n" +
                        errorMessage;
                }
                else
                {
                    _unitOfWork.Save();
                    TempData["success"] = "Consultatia a fost finalizata cu succes!";
                }

                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Input invalid.";
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
