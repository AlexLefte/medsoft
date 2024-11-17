using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using MedSoft.Models.ViewModels;
using MedSoft.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MedSoft.Areas.Medic.Controllers
{
    [Area("Medic")]
    [Authorize(Roles = $"{SD.Rol_Admin},{SD.Rol_Medic}")]
    public class MedicamentController : Controller
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        #endregion

        #region Constructor
        public MedicamentController(IUnitOfWork unitOfWork, 
            IWebHostEnvironment webHostEnvironment, 
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _userStore = userStore;
        }
        #endregion 

        #region Methods
        public async Task<IActionResult> Index()
        {
            List<Medicamente> listaMedicamenteVM = _unitOfWork.Medicamente.GetAll().ToList();
            return View(listaMedicamenteVM);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Medicamente medicament; 

            if (id != null)
            {
                medicament = _unitOfWork.Medicamente.Get(med => med.MedicamentID == id);              
            }
            else
            {
                medicament = new();
            }

            return View(medicament);
        }

        [HttpPost]
        public IActionResult Upsert(Medicamente medicament, IFormFile? file = null)
        {
            // Check whether the model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    if (medicament.MedicamentID != 0)
                    {
                        // Update the image if necessary
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string oldImage = medicament.ImageUrl ?? string.Empty;
                        if (!string.IsNullOrEmpty(oldImage))
                        {
                            oldImage = Path.Combine(wwwRootPath, oldImage.TrimStart('\\'));
                        }

                        if (file != null)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string imagePath = Path.Combine(wwwRootPath, @"images\medicamente", fileName);

                            if (!string.IsNullOrEmpty(oldImage))
                            {
                                if (System.IO.File.Exists(oldImage) && oldImage != @"\images\medicamente\No_image.jpg")
                                {
                                    System.IO.File.Delete(oldImage);
                                }
                            }

                            using (var fileStream = new FileStream(imagePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                            medicament.ImageUrl = @"\images\medicamente\" + fileName;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(oldImage) ||
                                !System.IO.File.Exists(oldImage))
                            {
                                medicament.ImageUrl = @"\images\medicamente\No_image.jpg";
                            }
                        }
                        // Actualizeaza medicament
                        _unitOfWork.Medicamente.Update(medicament);
                        _unitOfWork.Save();
                    }                      
                    else
                    {
                        // Adaugare url imagine
                        if (file != null)
                        {
                            string wwwRootPath = _webHostEnvironment.WebRootPath;
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string imagePath = Path.Combine(wwwRootPath, @"images\medicamente", fileName);

                            using (var fileStream = new FileStream(imagePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                            medicament.ImageUrl = @"\images\medicamente\" + fileName;
                        }
                        else
                        {
                            medicament.ImageUrl = @"\images\medicamente\No_image.jpg";
                        }

                        // Adauga medicament
                        _unitOfWork.Medicamente.Add(medicament);
                        _unitOfWork.Save();
                    }

                    TempData["succes"] = "Medicamentul a fost adaugat cu succes.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    TempData["error"] = "Medicamentul nu a putut fi adaugat.";
                    return View();
                }
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            Medicamente? medicamentFromDb = _unitOfWork.Medicamente.Get(med => med.MedicamentID == id);

            if (medicamentFromDb == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            return View(medicamentFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            Medicamente medicament = _unitOfWork.Medicamente.Get(med => med.MedicamentID == id);
            if (medicament == null)
            {
                return NotFound();
            }

            try
            {
                // Eliminare medicament
                _unitOfWork.Medicamente.Remove(medicament);
                _unitOfWork.Save();

                // Afisare rezultat afirmativ
                TempData["success"] = "Medicamentul a fost eliminat cu succes!";
            }
            catch (Exception e)
            {
                // Afisare rezultat negativ
                TempData["error"] = "Medicamentul nu a putut fi sters!";
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}
