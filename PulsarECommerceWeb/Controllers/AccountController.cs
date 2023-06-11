using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using PulsarECommerceData;
using PulsarECommerceWeb.Models;
using System.Security.Claims;

namespace PulsarECommerceWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment env;
        private readonly AppDbContext context;

        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IEmailService emailService,
            IWebHostEnvironment env,
            AppDbContext context
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailService = emailService;
            this.env = env;
            this.context = context;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel { IsPersistent = true });
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.IsPersistent, true);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı girişi!");
                return View(model);
            }


        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = new AppUser
            {
                UserName = model.UserName!,
                FirstName = model.FirstName!,
                LastName = model.LastName!,
                Gender = model.Gender,
                Enabled = true,
                EmailConfirmed = false
            };

            await userManager.CreateAsync(user, model.Password!);
            await userManager.AddToRoleAsync(user, "Members");
            await userManager.AddClaimAsync(user, new Claim("Name", user.Name));

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action("Confirm", "Account", new { id = user.Id, token }, Request.Scheme);
            await emailService.SendAsync(
                user.UserName,
                "E-Posta doğrulama mesajı",
                string.Format(
                    System.IO.File.ReadAllText(
                        Path.Combine(env.WebRootPath, "templates", "confirm.html")
                        )
                    , user.Name
                    , url),
                isHtml: true
                );

            return View("RegisterSuccess");
        }

        public async Task<IActionResult> Confirm(Guid id, string token)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            await userManager.ConfirmEmailAsync(user!, token);
            return View("Confirm");
        }

        public async Task<IActionResult> ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user is null)
            {
                ModelState.AddModelError("", "Geçersiz e-posta adresi!");
                return View(model);
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            await emailService.SendAsync(
                user.UserName,
                "E-Posta doğrulama mesajı",
                string.Format(
                    System.IO.File.ReadAllText(
                        Path.Combine(env.WebRootPath, "templates", "resetpassword.html")
                        ),
                    user.Name,
                    token),
                isHtml: true
                );


            return View("ResetPasswordForm", new ResetPasswordFormViewModel { UserId = user.Id });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordForm(ResetPasswordFormViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId.ToString());
            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return View("ResetPasswordSuccess");

            ModelState.AddModelError("", "Geçersiz kod!");
            return View(model);
        }

    }
}
