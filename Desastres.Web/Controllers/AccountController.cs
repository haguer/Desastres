using Microsoft.AspNetCore.Mvc;
using Desastres.Web.Helpers;
using Desastres.Web.Models;
using System.Linq;
using System.Threading.Tasks;
using Desastres.Web.Data.Entities;
using Desastres.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Desastres.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly DataContext _dataContext;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper,
             DataContext dataContext,
              IMailHelper mailHelper
            )
        {
            _userHelper = userHelper;
            _dataContext = dataContext;
             _mailHelper = mailHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Usuario o correo incorrecto");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var user = await AddUser(view);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Ese correo ya esta registrado");
                    return View(view);
                }

                var owner = new Encargado
                {
                    Usuarios = user,
                };

                _dataContext.Encargados.Add(owner);
                await _dataContext.SaveChangesAsync();


                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(view.Username, "Correo de confirmación", $"<h1>Correo de confirmación</h1>" +
                    $"Confirmar su usuario, " +
                    $"De click en este link: </br></br><a href = \"{tokenLink}\">Confirmar correo</a>");
                ViewBag.Message = "Las instrucciones fueron enviadas a su correo.";
                return View(view);

            }

            return View(view);
        }

        private async Task<Usuario> AddUser(AddUserViewModel view)
        {
            var user = new Usuario
            {
               
                Email = view.Username,
                Nombres = view.Nombres,
                Apellidos = view.Apellidos,
                PhoneNumber = view.PhoneNumber,
                UserName = view.Username
            };

            var result = await _userHelper.AddUserAsync(user, view.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserByEmailAsync(view.Username);
            await _userHelper.AddUserToRoleAsync(newUser, "Encargado");
            return newUser;
        }
        public async Task<IActionResult> ChangeUser()
        {
            var owner = await _dataContext.Encargados
                .Include(o => o.Usuarios)
                .FirstOrDefaultAsync(o => o.Usuarios.UserName.ToLower().Equals(User.Identity.Name.ToLower()));
            if (owner == null)
            {
                return NotFound();
            }

            var view = new EditUserViewModel
            {
               
                Nombres = owner.Usuarios.Nombres,
                Id = owner.Id,
                Apellidos = owner.Usuarios.Apellidos,
                PhoneNumber = owner.Usuarios.PhoneNumber
            };

            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var owner = await _dataContext.Encargados
                    .Include(o => o.Usuarios)
                    .FirstOrDefaultAsync(o => o.Id == view.Id);

                owner.Usuarios.Nombres = view.Nombres;
                owner.Usuarios.Apellidos = view.Apellidos;               
                owner.Usuarios.PhoneNumber = view.PhoneNumber;

                await _userHelper.UpdateUserAsync(owner.Usuarios);
                return RedirectToAction("Index", "Home");
            }

            return View(view);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                }
            }

            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "El correo no corresponde al registrado.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(model.Email, "Reiniciar Contraseña", $"<h1>Reiniciar Contraseña</h1>" +
                    $"Para reiniciar contraseña click en el link:</br></br>" +
                    $"<a href = \"{link}\">Resetear Contraseña</a>");
                ViewBag.Message = "Las instrucciones para recuperar la contraseña fueron enviadas al email.";
                return View();

            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Contraseña cambiada correctamente.";
                    return View();
                }

                ViewBag.Message = "Error al cambiar la contraseña.";
                return View(model);
            }

            ViewBag.Message = "Usuario no encontrado.";
            return View(model);
        }

    }
}
