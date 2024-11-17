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
    public class PacientController : Controller
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        #endregion

        #region Constructor
        public PacientController(IUnitOfWork unitOfWork, 
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
            List<PacientVM> listaPacientiVM = await GetAllVM();
            return View(listaPacientiVM);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {
            PacientVM pacientVM = new()
            {
                Pacient = new()
            };
            if (id != null && id != "undefined")
            {
                // Update
                pacientVM.Pacient = _unitOfWork.Pacient.Get(p => p.PacientID == id);
                pacientVM.User = await _userManager.FindByIdAsync(id);
                pacientVM.Email = pacientVM.User.Email;
                pacientVM.NumarTelefon = pacientVM.User.PhoneNumber.ToString();           
            }
            return View(pacientVM);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(PacientVM pacientVM)
        {
            // Set the rollback flag
            bool commit = true;

            // Check whether the model is valid
            if (ModelState.IsValid)
            {
                if (pacientVM.Pacient.PacientID != null && pacientVM.Pacient.PacientID != string.Empty)
                {
                    using (var transaction = _unitOfWork.DbContext.Database.BeginTransaction())
                    {
                        // Update
                        // 1. Update the user
                        var user = await _userManager.FindByIdAsync(pacientVM.Pacient.PacientID);
                        if (user != null)
                        {
                            user.UserName = pacientVM.Email;
                            user.PhoneNumber = pacientVM.NumarTelefon;
                            user.Email = pacientVM.Email;

                            // Verifica daca a fost introdusa o noua parola
                            if (!string.IsNullOrEmpty(pacientVM.NewPassword))
                            {
                                // Reseteaza parola:
                                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                                var passResult = await _userManager.ResetPasswordAsync(user, token, pacientVM.NewPassword);

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
                                    // 2. Update the Pacient
                                    try
                                    {                                    
                                        _unitOfWork.Pacient.Update(pacientVM.Pacient);
                                        _unitOfWork.Save();
                                    }
                                    catch (DbUpdateException ex)
                                    {
                                        // Pacientul nu a putut fi actualizat
                                        if (ex.InnerException is SqlException sqlEx)
                                        {
                                            // Check for unique constraint violation error numbers
                                            if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                                            {
                                                commit = false;
                                                TempData["error"] = "Pacientul nu a putut fi adaugat: CNP-ul exista deja.";
                                            }
                                        }
                                        else
                                        {
                                            commit = false;
                                            TempData["error"] = "Datele Pacientului nu au putut fi actualizate.";
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
                            user.UserName = pacientVM.Email;
                            await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
                            await _emailStore.SetEmailAsync(user, pacientVM.Email, CancellationToken.None);
                            user.PhoneNumber = pacientVM.NumarTelefon;
                            user.Email = pacientVM.Email;

                            result = await _userManager.CreateAsync(user, pacientVM.NewPassword);

                            if (result.Succeeded)
                            {
                                // Adauga rol => Pacient
                                await _userManager.AddToRoleAsync(user, "Pacient");

                                // Seteaza cheia => ID-ul user-ului creat
                                pacientVM.Pacient.PacientID = user.Id;

                                _unitOfWork.Pacient.Add(pacientVM.Pacient);
                                _unitOfWork.Save();
                                TempData["success"] = "Pacientul a fost adaugat cu succes!";
                            }
                            else
                            {
                                TempData["error"] = "Pacientul nu a putut fi creat: " + 
                                    result.ToString();
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
                                    TempData["error"] = "Pacientul nu a putut fi adaugat: exista deja.";
                                }
                            }
                            TempData["error"] = "Pacientul nu a putut fi adaugat";
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
                            return View(pacientVM);
                        }
                    }
                }                      
            }

            return View(pacientVM);
        }

        /*[HttpPost]
        public async Task<IActionResult> Add(MedicVM medic, IFormFile? file = null)
        {
            // Check whether the model is valid
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                IdentityResult result = new IdentityResult();
                try
                {
                    await _userStore.SetUserNameAsync(user, medic.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, medic.Email, CancellationToken.None);
                    user.PhoneNumber = medic.NumarTelefon;
                    user.UserName = $"{medic.Medic.NumeMedic} {medic.Medic.PrenumeMedic}";

                    result = await _userManager.CreateAsync(user, medic.Password);

                    if (result.Succeeded)
                    {
                        if (file != null)
                        {
                            string wwwRootPath = _webHostEnvironment.WebRootPath;
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string medicPath = Path.Combine(wwwRootPath, @"images\medici", fileName);

                            using (var fileStream = new FileStream(medicPath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                            medic.Medic.ImageUrl = @"\images\medici\" + fileName;
                        }
                        else
                        {
                            medic.Medic.ImageUrl = @"\images\medici\No_image.png";
                        }
                    
                        _unitOfWork.Medic.Add(medic.Medic);
                        _unitOfWork.Save();

                        await _userManager.AddToRoleAsync(user, "Medic");
                    }
                }
                catch (DbUpdateException ex)
                {
                    if (result.Succeeded)
                    {
                        // Delete the user
                        var resultDelete = await _userManager.DeleteAsync(user);
                    }

                    if (ex.InnerException is SqlException sqlEx)
                    {
                        // Check for unique constraint violation error numbers
                        if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                        {
                            // Unique constraint violation handling
                            TempData["error"] = "Medicul nu a putut fi adaugat: exista deja.";
                            return RedirectToAction("Index");
                        }
                    }
                    TempData["error"] = "Medicul nu a putut fi adaugat";
                    return RedirectToAction("Index");
                }

                TempData["success"] = "Medicul a fost adaugat cu succes!";
                // Return to the speciality list:
                return RedirectToAction("Index");
                // return RedirectToAction("Index", "Category");
            }

            return View();
        }*/

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

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || id == string.Empty)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            PacientVM? pacientFromDb = await GetPacientVM(id);

            if (pacientFromDb == null)
            {
                TempData["error"] = "Nu a fost gasit";
                return RedirectToAction("Index");
            }
            return View(pacientFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(string? id)
        {
            Models.Pacient? pacient = _unitOfWork.Pacient.Get(cat => cat.PacientID == id);
            if (pacient == null)
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
                        // Eliminare pacient
                        _unitOfWork.Pacient.Remove(pacient);
                        _unitOfWork.Save();

                        // Eliminare utilizator
                        await _userManager.DeleteAsync(user);

                        // Commit
                        transaction.Commit();

                        // Afisare confirmare
                        TempData["success"] = "Pacientul a fost eliminat cu succes!";

                        // Return to the categories list:
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    // Rollback
                    transaction.Rollback();

                    // Afiseaza mesaj de eroare
                    TempData["error"] = "Pacientul nu a putut fi sters!";
                    return await Index();
                }
            }

            return await Index();
        }
        #endregion

        #region Utils
        private async Task<PacientVM?> GetPacientVM(string pacientID)
        {
            Models.Pacient? p = _unitOfWork.Pacient.Get(p => p.PacientID == pacientID);

            if (p != null)
            {
                PacientVM pacientVM = new()
                {
                    Pacient = p,
                    User = await _userManager.FindByIdAsync(p.PacientID)
                };
                pacientVM.Email = pacientVM.User.Email;
                pacientVM.NumarTelefon = pacientVM.User.PhoneNumber.ToString();

                return pacientVM;
            }

            return null;
        }

        public async Task<List<PacientVM>> GetAllVM()
        {
            List<PacientVM> listaPacientiVM = new List<PacientVM>();
            foreach (Models.Pacient p in _unitOfWork.Pacient.GetAll().ToList())
            {
                PacientVM pacientVM = new()
                {
                    Pacient = p,
                    User = await _userManager.FindByIdAsync(p.PacientID)
                };
                pacientVM.Email = pacientVM.User.Email;
                pacientVM.NumarTelefon = pacientVM.User.PhoneNumber.ToString();
                listaPacientiVM.Add(pacientVM);
            }
            return listaPacientiVM;
        }
        #endregion
    }
}
