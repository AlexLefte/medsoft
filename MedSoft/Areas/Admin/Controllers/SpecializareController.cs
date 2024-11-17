using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedSoft.Utility;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace MedSoft.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Rol_Admin)]
    public class SpecializareController : Controller
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructors
        public SpecializareController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Methods
        static string ToTitleCase(string input)
        {
            if (input != null)
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                return textInfo.ToTitleCase(input.ToLower());
            }

            return string.Empty;
        }

        public IActionResult Index()
        {
            List<Specializare> listaSpecializari = _unitOfWork.Specializare.GetAll().ToList();
            foreach (Specializare specializare in listaSpecializari)
            {
                specializare.NumarMedici = _unitOfWork.Specializare.CountMedici(specializare.SpecializareID);
            }
            return View(listaSpecializari);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Specializare specializare)
        {
            // Check whether the model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    specializare.Nume = ToTitleCase(specializare.Nume);
                    _unitOfWork.Specializare.Add(specializare);
                    _unitOfWork.Save();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx)
                    {
                        // Check for unique constraint violation error numbers
                        if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                        {
                            // Unique constraint violation handling
                            TempData["error"] = "Specializarea nu a putut fi adaugata: Specialitatea exista deja.";
                            return RedirectToAction("Index");
                        }
                    }
                    TempData["error"] = "Specializarea nu a putut fi adaugata";
                    return RedirectToAction("Index");
                }
                
                TempData["success"] = "Specializarea a fost creata cu succes!";
                // Return to the speciality list:
                return RedirectToAction("Index");
                // return RedirectToAction("Index", "Category");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            Specializare? specializareFromDb = _unitOfWork.Specializare.Get(cat => cat.SpecializareID == id);

            if (specializareFromDb == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            return View(specializareFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Specializare specializare)
        {
            // Check whether the model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    specializare.Nume = ToTitleCase(specializare.Nume);
                    _unitOfWork.Specializare.Update(specializare);
                    _unitOfWork.Save();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx)
                    {
                        // Check for unique constraint violation error numbers
                        if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                        {
                            // Unique constraint violation handling
                            TempData["error"] = "Specializarea nu a putut fi modificata: Specialitatea exista deja.";
                            return RedirectToAction("Index");
                        }
                    }
                    TempData["error"] = "Specializarea nu a putut fi modificata.";
                    return RedirectToAction("Index");
                }

                TempData["success"] = "Specializarea a fost actualizata cu succes!";
                // Return to the speciality list:
                return RedirectToAction("Index");
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
            Specializare? specializareFromDb = _unitOfWork.Specializare.Get(cat => cat.SpecializareID == id);

            if (specializareFromDb == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            return View(specializareFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Specializare? obj = _unitOfWork.Specializare.Get(cat => cat.SpecializareID == id);
            if (obj == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }

            // Check whether the model is valid
            if (ModelState.IsValid)
            {
                _unitOfWork.Specializare.Remove(obj);
                _unitOfWork.Save();

                TempData["success"] = "Specializarea a fost eliminata cu succes!";

                // Return to the categories list:
                return RedirectToAction("Index");
            }

            return View();
        }
        #endregion
    }
}
