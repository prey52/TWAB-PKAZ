﻿// Licensed to the .NET Foundation under one or more agreements.
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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace TWAB.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<DBUser> _signInManager;
        private readonly UserManager<DBUser> _userManager;
        private readonly IUserStore<DBUser> _userStore;
        private readonly IUserEmailStore<DBUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<DBUser> userManager,
            IUserStore<DBUser> userStore,
            SignInManager<DBUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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
            // Istniejące pola...
            [Required]
            [StringLength(20, ErrorMessage = "Przekroczono maksymalną ilość znaków")]
            [Display(Name = "Imię")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(20, ErrorMessage = "Przekroczono maksymalną ilość znaków")]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Data urodzenia")]
            public DateTime BirthDate { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(9, ErrorMessage = "Numer telefonu składa się z 9 znaków.", MinimumLength = 9)]
            [Display(Name = "Numer telefonu")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Rola")]
            public string Role { get; set; }

            [Display(Name = "Nazwa firmy")]
            public string CompanyName { get; set; }

            [Display(Name = "Logo firmy")]
            public IFormFile CompanyLogo { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Powtórz hasło")]
            [Compare("Password", ErrorMessage = "Hasła różnią się od siebie.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.BirthDate = Input.BirthDate;
                user.PhoneNumber = Input.PhoneNumber;
                user.BirthDate = Input.BirthDate;
                //na wypadek gdyby użytkownik zmienił typ na "rekruter", uzupełnił wartości i przełączył z powrotem na "użytkownik"
                if (Input.Role == "Recruiter")
                {
                    if (Input.CompanyName == null || Input.CompanyLogo == null)
                    {
                        return Page();
                    }
                    await _userManager.AddToRoleAsync(user, "Recruiter");
                    user.CompanyName = Input.CompanyName;
                    if (Input.CompanyLogo != null && Input.CompanyLogo.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await Input.CompanyLogo.CopyToAsync(memoryStream);
                            user.CompanyLogo = memoryStream.ToArray();
                        }
                    }
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

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
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private DBUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<DBUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(DBUser)}'. " +
                    $"Ensure that '{nameof(DBUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<DBUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<DBUser>)_userStore;
        }
    }
}