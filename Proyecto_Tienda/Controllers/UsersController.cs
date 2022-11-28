using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Proyecto_Tienda.Models;
using Proyecto_Tienda.Services;

namespace Proyecto_Tienda.Controllers
{
    public class UsersController: Controller {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;

        public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        // This allows access to everybody
        [AllowAnonymous]
        public IActionResult Register () {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register (RegisterViewModel model) {
            if (!ModelState.IsValid) { 
                return View(model);
            }

            var usuario = new IdentityUser() { Email = model.Email, UserName = model.Email };
            var resultado = await userManager.CreateAsync(usuario, password: model.Password);

            if (resultado.Succeeded) {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else {
                foreach (var error in resultado.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string mensaje = null) {
            if (mensaje is not null) {
                ViewData["mensaje"] = mensaje;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var resultado = await signInManager.PasswordSignInAsync(model.Email,
                model.Password, model.RememberMe, lockoutOnFailure: false);

            if (resultado.Succeeded) {
                return RedirectToAction("Index", "Home");
            }
            else {
                ModelState.AddModelError(string.Empty, "Username or password incorrect");
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult ExternalLogin (string proveedor, string returnUrl = null) {
            var urlRedireccion = Url.Action("RegisterExternalUser", values: new { returnUrl });
            var propiedades = signInManager
                .ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);
            return new ChallengeResult(proveedor, propiedades);
        }


        [AllowAnonymous]
        public async Task<IActionResult> RegisterExternalUser (string urlRetorno = null, string remoteError = null) {
            urlRetorno = urlRetorno ?? Url.Content("~/");
            var mensaje = "";

            if (remoteError is not null) {
                mensaje = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null) {
                mensaje = "Error cargando la data de login externo";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            // Ya la cuenta existe
            if (resultadoLoginExterno.Succeeded) {
                return LocalRedirect(urlRetorno);
            }

            string email = "";

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email)) {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else {
                mensaje = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var usuario = new IdentityUser { Email = email, UserName = email };
            var resultadoCrearUsuario = await userManager.CreateAsync(usuario);

            if (!resultadoCrearUsuario.Succeeded) {
                mensaje = resultadoCrearUsuario.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, info);

            if (resultadoAgregarLogin.Succeeded) {
                await signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }

            mensaje = "Ha ocurrido un error agregando el login";
            return RedirectToAction("login", routeValues: new { mensaje });
        }


        [HttpGet]
        [Authorize(Roles = Constants.RolAdmin)]
        public async Task<IActionResult> Listed (string mensaje = null) {
            var users = await context.Users.Select(u => new UserViewModel {
                Email = u.Email
            }).ToListAsync();

            var model = new UserListViewModel();
            model.Users = users;
            model.Mensaje = mensaje;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Constants.RolAdmin)]
        public async Task<IActionResult> GrantAdmin (string email) {
            var user = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user is null) {
                return NotFound();
            }

            await userManager.AddToRoleAsync(user, Constants.RolAdmin);

            return RedirectToAction("Listed",
                routeValues: new { mensaje = "Rol asignado correctamente a " + email });
        }


        [HttpPost]
        [Authorize(Roles = Constants.RolAdmin)]
        public async Task<IActionResult> RevokeAdmin (string email) {
            var user = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user is null) {
                return NotFound();
            }

            await userManager.RemoveFromRoleAsync(user, Constants.RolAdmin);

            return RedirectToAction("Listado",
                routeValues: new { mensaje = "Rol removido correctamente a " + email });
        }

    }
}
