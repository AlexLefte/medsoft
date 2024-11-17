using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using MedSoft.Models.ViewModels;
using MedSoft.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.CodeAnalysis.Differencing;

namespace MedSoft.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Rol_Admin)]
    public class MedicController : Controller
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        #endregion

        #region Constructor
        public MedicController(IUnitOfWork unitOfWork, 
            IWebHostEnvironment webHostEnvironment, 
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
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

        public async Task<IActionResult> Index()
        {
            // List<Medic> listaMedici = _unitOfWork.Medic.GetAll(includeProperties:"Specializare").ToList();
            List<MedicVM> listaMediciVM = new List<MedicVM>();
            foreach (Models.Medic med in _unitOfWork.Medic.GetAll(includeProperties: "Specializare").ToList())
            {
                MedicVM medicVM = new()
                {
                    ListaSpecializari = _unitOfWork.Specializare.GetAll().
                    Select(cat => new SelectListItem
                    {
                        Text = cat.Nume,
                        Value = cat.SpecializareID.ToString()
                    }).ToList(),
                    Medic = med,
                    User = await _userManager.FindByIdAsync(med.MedicID)
                };
                medicVM.Email = medicVM.User.Email;
                medicVM.NumarTelefon = medicVM.User.PhoneNumber.ToString();
                listaMediciVM.Add(medicVM);
            }
            return View(listaMediciVM);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {
            MedicVM medicVM = new()
            {
                ListaSpecializari = _unitOfWork.Specializare.GetAll().
                Select(cat => new SelectListItem
                {
                    Text = cat.Nume,
                    Value = cat.SpecializareID.ToString()
                }).ToList(),
                Medic = new()
            };
            if (id != null && id != "undefined")
            {
                // Update
                medicVM.Medic = _unitOfWork.Medic.Get(med => med.MedicID == id);
                medicVM.User = await _userManager.FindByIdAsync(id);
                medicVM.Email = medicVM.User.Email;
                medicVM.NumarTelefon = medicVM.User.PhoneNumber.ToString();
            }
            return View(medicVM);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(MedicVM medicVM, IFormFile? file = null)
        {
            // Set the rollback flag
            bool commit = true;

            // Check whether the model is valid
            if (ModelState.IsValid)
            {
                if (medicVM.Medic.MedicID != null && medicVM.Medic.MedicID != string.Empty)
                {
                    using (var transaction = _unitOfWork.DbContext.Database.BeginTransaction())
                    {
                        // Update
                        // 1. Update the user
                        var user = await _userManager.FindByIdAsync(medicVM.Medic.MedicID);
                        if (user != null)
                        {
                            user.UserName = medicVM.Medic.PrenumeMedic;
                            user.PhoneNumber = medicVM.NumarTelefon;
                            user.Email = medicVM.Email;
                            // Verifica daca a fost introdusa o noua parola
                            if (!string.IsNullOrEmpty(medicVM.NewPassword))
                            {
                                // Reseteaza parola:
                                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                                var passResult = await _userManager.ResetPasswordAsync(user, token, medicVM.NewPassword);

                                if (passResult.Errors.Any())
                                {
                                    // Utilizatorul nu a fost gasit
                                    commit = false;
                                    TempData["error"] = "Parola nu a putut fi actualizata.";
                                }
                            }

                            if (commit)
                            {
                                var result = await _userManager.UpdateAsync(user);
                                if (result.Succeeded)
                                {
                                    // 2. Update the medic
                                    try
                                    {
                                        // Update the image if necessary
                                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                                        string oldImage = medicVM.Medic.ImageUrl ?? string.Empty;
                                        if (!string.IsNullOrEmpty(oldImage))
                                        {
                                            oldImage = Path.Combine(wwwRootPath, oldImage.TrimStart('\\'));
                                        }

                                        if (file != null)
                                        {
                                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                            string imagePath = Path.Combine(wwwRootPath, @"images\medici", fileName);

                                            if (!string.IsNullOrEmpty(oldImage))
                                            {
                                                if (System.IO.File.Exists(oldImage))
                                                {
                                                    System.IO.File.Delete(oldImage);
                                                }
                                            }

                                            using (var fileStream = new FileStream(imagePath, FileMode.Create))
                                            {
                                                file.CopyTo(fileStream);
                                            }
                                            medicVM.Medic.ImageUrl = @"\images\medici\" + fileName;
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(oldImage) ||
                                                !System.IO.File.Exists(oldImage))
                                            {
                                                medicVM.Medic.ImageUrl = @"\images\medici\No_image.png";
                                            }
                                        }

                                        _unitOfWork.Medic.Update(medicVM.Medic);
                                        _unitOfWork.Save();
                                    }
                                    catch (DbUpdateException ex)
                                    {
                                        // Medicul nu a putut fi actualizat
                                        if (ex.InnerException is SqlException sqlEx)
                                        {
                                            // Check for unique constraint violation error numbers
                                            if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                                            {
                                                // Unique constraint violation handling
                                                commit = false;
                                                TempData["error"] = "Medicul nu a putut fi adaugat: CNP-ul exista deja.";
                                            }
                                        }
                                        else
                                        {
                                            // Alte motive
                                            commit = false;
                                            TempData["error"] = "Datele medicului nu au putut fi actualizate.";
                                        }
                                    }
                                }
                                else
                                {
                                    // Utilizatorul nu a fost gasit
                                    commit = false;
                                    TempData["error"] = "Utilizatorul nu a putut fi actualizat.";
                                }
                            }
                        }
                        else
                        {
                            // Utilizatorul nu a fost gasit
                            commit = false;
                            TempData["error"] = "Utilizatorul nu a putut fi gasit.";
                        }

                        if (commit)
                        {
                            // If both updates succeeded => Commit
                            transaction.Commit();
                            TempData["success"] = "Utilizatorul a fost utilizat cu succes.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Rollback
                            transaction.Rollback();
                            return View();
                        }
                    }
                }
                else
                {
                    using (var transaction = _unitOfWork.DbContext.Database.BeginTransaction())
                    {
                        // Add
                        var user = CreateUser();
                        IdentityResult result = new IdentityResult();
                        try
                        {
                            user.UserName = medicVM.Email;
                            await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
                            await _emailStore.SetEmailAsync(user, medicVM.Email, CancellationToken.None);
                            user.PhoneNumber = medicVM.NumarTelefon;
                            user.Email = medicVM.Email;

                            result = await _userManager.CreateAsync(user, medicVM.NewPassword);

                            if (result.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(user, "Medic");

                                // Ad the image url the image if necessary
                                if (file != null)
                                {
                                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                    string medicPath = Path.Combine(wwwRootPath, @"images\medici", fileName);

                                    using (var fileStream = new FileStream(medicPath, FileMode.Create))
                                    {
                                        file.CopyTo(fileStream);
                                    }
                                    medicVM.Medic.ImageUrl = @"\images\medici\" + fileName;
                                }
                                else
                                {
                                    medicVM.Medic.ImageUrl = @"\images\medici\No_image.jpg";
                                }

                                // Seteaza cheia => ID-ul user-ului creat
                                medicVM.Medic.MedicID = user.Id;

                                _unitOfWork.Medic.Add(medicVM.Medic);
                                _unitOfWork.Save();
                                TempData["success"] = "Medicul a fost adaugat cu succes!";
                            }
                        }
                        catch (DbUpdateException ex)
                        {
                            commit = false;

                            if (ex.InnerException is SqlException sqlEx)
                            {
                                // Check for unique constraint violation error numbers
                                if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                                {
                                    // Unique constraint violation handling
                                    TempData["error"] = "Medicul nu a putut fi adaugat: exista deja.";
                                }
                            }
                            TempData["error"] = "Medicul nu a putut fi adaugat";
                        }
                        
                        if (commit)
                        {
                            // Both the user and the medic were inserted => commit
                            transaction.Commit();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Rollback
                            transaction.Rollback();
                            return View(medicVM);
                        }
                    }
                }                      
            }

            return View(medicVM);
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }        

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || id == string.Empty)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            MedicVM? medicFromDb = await GetMedicVM(id);

            if (medicFromDb == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            return View(medicFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(string? id)
        {
            Models.Medic? medic = _unitOfWork.Medic.Get(cat => cat.MedicID == id);
            if (medic == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }

            IdentityUser? user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }

            using (var transaction = _unitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Check whether the model is valid
                    if (ModelState.IsValid)
                    {
                        // Eliminare medic
                        _unitOfWork.Medic.Remove(medic);
                        _unitOfWork.Save();

                        // Eliminare utilizator
                        await _userManager.DeleteAsync(user);

                        // Commit
                        transaction.Commit();

                        // Afisare confirmare
                        TempData["success"] = "Medicul a fost eliminat cu succes!";

                        // Return to the categories list:
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    // Rollback
                    transaction.Rollback();

                    // Afiseaza mesaj de eroare
                    TempData["error"] = "Medicul nu a putut fi sters!";
                    return RedirectToAction("Index");
                }
            }

            return await Index();
        }
        #endregion

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Models.Medic> listaMedici = _unitOfWork.Medic.GetAll(includeProperties: "Specializare").ToList();
            return Json(new { data = listaMedici });
        }

        /*[HttpGet]
        public async Task<IActionResult> GetAllVM()
        {
            List<MedicVM> listaMediciVM = new List<MedicVM>();
            foreach (Medic med in _unitOfWork.Medic.GetAll(includeProperties: "Specializare").ToList())
            {
                MedicVM medicVM = new()
                {
                    ListaSpecializari = _unitOfWork.Specializare.GetAll().
                    Select(cat => new SelectListItem
                    {
                        Text = cat.Nume,
                        Value = cat.SpecializareID.ToString()
                    }).ToList(),
                    Medic = med,
                    User = await _userManager.FindByIdAsync(med.MedicID)
                };
                medicVM.Email = medicVM.User.Email;
                medicVM.NumarTelefon = medicVM.User.PhoneNumber.ToString();
                listaMediciVM.Add(medicVM);
            }
            return Json(new { data = listaMediciVM });
        }*/

        /*[HttpDelete]
        public IActionResult Delete(string? id)
        {
            var medicToBeDeleted = _unitOfWork.Medic.Get(product => product.MedicID == id);
            if (medicToBeDeleted == null)
            {
                return Json(new { success = false, message = "Eroare!" });
            }

            string oldImage = medicToBeDeleted.ImageUrl;
            if (!string.IsNullOrEmpty(oldImage))
            {
                // Remove the old image:
                oldImage = Path.Combine(_webHostEnvironment.WebRootPath, oldImage.TrimStart('\\'));

                if (System.IO.File.Exists(oldImage))
                {
                    System.IO.File.Delete(oldImage);
                }
            }

            _unitOfWork.Medic.Remove(medicToBeDeleted);
            _unitOfWork.Save();

            List<Medic> listaMedici = _unitOfWork.Medic.GetAll(includeProperties: "Specializare").ToList();
            return Json(new { data = medicToBeDeleted, success = true, message = "Medicul a fost sters cu succes!" });
        }*/
        #endregion

        #region Utils
        private async Task<MedicVM?> GetMedicVM(string medicID)
        {
            Models.Medic? med = _unitOfWork.Medic.Get(medic => medic.MedicID == medicID);

            if (med != null)
            {
                MedicVM medicVM = new()
                {
                    ListaSpecializari = _unitOfWork.Specializare.GetAll().
                    Select(cat => new SelectListItem
                    {
                        Text = cat.Nume,
                        Value = cat.SpecializareID.ToString()
                    }).ToList(),
                    Medic = med,
                    User = await _userManager.FindByIdAsync(med.MedicID)
                };
                medicVM.Email = medicVM.User.Email;
                medicVM.NumarTelefon = medicVM.User.PhoneNumber.ToString();

                return medicVM;
            }

            return null;
        }

        public async Task<List<MedicVM>> GetAllVM()
        {
            List<MedicVM> listaMediciVM = new List<MedicVM>();
            foreach (Models.Medic med in _unitOfWork.Medic.GetAll(includeProperties: "Specializare").ToList())
            {
                MedicVM medicVM = new()
                {
                    ListaSpecializari = _unitOfWork.Specializare.GetAll().
                    Select(cat => new SelectListItem
                    {
                        Text = cat.Nume,
                        Value = cat.SpecializareID.ToString()
                    }).ToList(),
                    Medic = med,
                    User = await _userManager.FindByIdAsync(med.MedicID)
                };
                medicVM.Email = medicVM.User.Email;
                medicVM.NumarTelefon = medicVM.User.PhoneNumber.ToString();
                listaMediciVM.Add(medicVM);
            }
            return listaMediciVM;
        }
        #endregion
    }
}
