// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MedSoft.DataAccess.Repository.IRepository;
using MedSoft.Models;
using MedSoft.Utility;
using Microsoft.AspNetCore.Hosting;
using Stripe;

namespace MedSoft.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [StringLength(100, ErrorMessage = "{0} trebuie sa contina cel putin {2} si maximum {1} caractere.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     User role property
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [DataType(DataType.Text)]
            [Display(Name = "Rol")]
            public string Role { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

            /// <summary>
            ///  Nume
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [DataType(DataType.Text)]
            public string? Nume { get; set; }

            /// <summary>
            ///  Prenume
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [DataType(DataType.Text)]
            public string? Prenume { get; set; }

            /// <summary>
            ///  Adresa
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [DataType(DataType.Text)]
            public string? Adresa { get; set; }

            /// <summary>
            /// Phone Number
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Numar de telefon")]
            [RegularExpression(@"^(\+\d{1,2}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage ="Numar de telefon invalid")]
            public string NumarTelefon { get; set; }

                 
            /// <summary>
            /// Cod Numeric Personal
            /// </summary>
            [Required(ErrorMessage = "Camp obligatoriu")]
            [DataType(DataType.Text)]
            [StringLength(13)]
            [RegularExpression(@"^\d{13}$", ErrorMessage ="CNP invalid")]
            public string? CNP { get; set; }

            /// <summary>
            /// Asigurare
            /// </summary>
            [DataType(DataType.Text)]
            [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Format numeric invalid")]
            public double? Asigurare { get; set; }

            /// <summary>
            /// Pretul consultatiei
            /// </summary>
            [DataType(DataType.Text)]
            public double? PretConsultatie { get; set; }

            /// <summary>
            /// The Image Url
            /// </summary>
            [DataType(DataType.Text)]
            public string? ImageUrl { get; set; }

            /// <summary>
            /// Specializare
            /// </summary>
            [DataType(DataType.Text)]
            public int? Specializare {  get; set; }

            [ValidateNever]
            /// <summary>
            /// Lista de specializari
            /// </summary>
            public IEnumerable<SelectListItem> ListaSpecializari { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            Input = new()
            {
                RoleList = _roleManager.Roles.Select(role => role.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                ListaSpecializari = _unitOfWork.Specializare.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Nume,
                    Value = i.SpecializareID.ToString()
                })
            };

            var list = Input.RoleList;
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, IFormFile? file = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Creeaza utilizatorul de baza
                var user = CreateUser();
                user.UserName = Input.Email;
                await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.PhoneNumber = Input.NumarTelefon;
                user.UserName = Input.Email;

                IdentityResult result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {
                    bool success = false;
                    // Creeaza entitatea de tip Pacient/Medic in functie de rolul ales
                    // _userManager.DeleteAsync(user);
                    try
                    {
                        switch (Input.Role)
                        {
                            case "Admin":
                                Administrator admin = new Administrator()
                                {
                                    AdministratorID = user.Id,
                                    Nume = Input.Nume,
                                    Prenume = Input.Prenume,
                                    CNP = Input.CNP,
                                    Adresa = Input.Adresa
                                };
                                _unitOfWork.Administrator.Add(admin);
                                break;
                            case "Medic":                            
                                Models.Medic medic = new Models.Medic()
                                {
                                    MedicID = user.Id,
                                    NumeMedic = Input.Nume,
                                    PrenumeMedic = Input.Prenume,
                                    PretConsultatie = (decimal)Input.PretConsultatie,
                                    SpecializareID = (int)Input.Specializare,
                                    CNP = Input.CNP,
                                    Adresa = Input.Adresa
                                };

                                if (file != null)
                                {
                                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                    string medicPath = Path.Combine(wwwRootPath, @"images\medici", fileName);

                                    using (var fileStream = new FileStream(medicPath, FileMode.Create))
                                    {
                                        file.CopyTo(fileStream);
                                    }
                                    medic.ImageUrl = @"\images\medici\" + fileName;
                                }
                                else
                                {
                                    medic.ImageUrl = @"\images\medici\No_image.png";
                                }

                                _unitOfWork.Medic.Add(medic);
                                break;
                            case "Pacient":
                                Models.Pacient pacient = new Models.Pacient()
                                {
                                    PacientID = user.Id,
                                    NumePacient = Input.Nume,
                                    PrenumePacient = Input.Prenume,
                                    CNP = Input.CNP,
                                    Adresa = Input.Adresa,
                                    Asigurare = (decimal)Input.Asigurare
                                };

                                _unitOfWork.Pacient.Add(pacient);
                                break;
                        }

                        success = true;
                    }
                    catch
                    {
                        _logger.LogError($"Could not create {Input.Role}");
                    }

                    if (success)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        if (!string.IsNullOrEmpty(Input.Role))
                        {
                            await _userManager.AddToRoleAsync(user, Input.Role);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, SD.Rol_Pacient);
                        }


                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    else
                    {
                        var resultDelete = await _userManager.DeleteAsync(user);
                        _logger.LogError($"Deleted user.");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }     
            }

            // If we got this far, something failed, redisplay form
            return Page();
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
    }
}
