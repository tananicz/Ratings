using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratings.Models;
using System.Text.RegularExpressions;

namespace Ratings.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr) 
        {
            _userMgr = userMgr;
            _signInManager = signInMgr;
        }

        public IActionResult Login([FromQuery] string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View(new UserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PerformLogin(UserViewModel userModel, string returnUrl)
        {
            ModelState.Remove(nameof(UserViewModel.CofirmPassword));
            ModelState.Remove(nameof(UserViewModel.AcceptRules));

            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(userModel.UserName, userModel.Password, false, false);
                if (result.Succeeded)
                {
                    return String.IsNullOrEmpty(returnUrl) ? RedirectToAction("Ratings", "Index") : Redirect(returnUrl);
                }
                ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika lub hasło");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View("Login", userModel);
        }

        public IActionResult CreateAccount([FromQuery] string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View(new UserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PerformCreate(UserViewModel userModel, string returnUrl)
        {
            if (await DoesUserExist(userModel.UserName))
            {
                ModelState.AddModelError(nameof(UserViewModel.UserName), $"W serwisie już istnieje użytkownik {userModel.UserName}");
            }

            if (!IsPasswordValid(userModel.Password))
            {
                ModelState.AddModelError(nameof(UserViewModel.Password), "Hasło powinno zawierać przynajmniej jedną wielką literę, jedną małą literę, jedną cyfrę i jeden znak specjalny: _ @ # $ % ^ & *");
            }
            
            if (String.IsNullOrEmpty(userModel.CofirmPassword))
            {
                ModelState.AddModelError(nameof(UserViewModel.CofirmPassword), "Musisz potwierdzić hasło");
            } 
            else if (!userModel.Password.Equals(userModel.CofirmPassword))
            {
                ModelState.AddModelError(nameof(UserViewModel.CofirmPassword), "Oba hasła muszą być takie same");
            }

            if (!userModel.AcceptRules)
            {
                ModelState.AddModelError(nameof(UserViewModel.AcceptRules), "Musisz zaakceptować regulamin serwisu");
            }

            if (ModelState.IsValid)
            {
                IdentityUser newUser = new IdentityUser(userModel.UserName);
                IdentityResult result = await _userMgr.CreateAsync(newUser, userModel.Password);
                if (result.Succeeded)
                {
                    result = await _userMgr.AddToRoleAsync(newUser, "user");
                    if (result.Succeeded)
                    {
                        ViewBag.ReturnUrl = returnUrl;
                        return View(new UserViewModel
                        {
                            UserName = userModel.UserName
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Nie udało się przypisać nowego użytkownika do roli 'user'");
                    }
                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View("CreateAccount", userModel);
        }

        public async Task<IActionResult> Logout([FromQuery] string ReturnUrl)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Ratings");
        }

        [Authorize(Roles = "admin")]
        public IActionResult ShowUsers()
        {
            return View(_userMgr.Users.ToList());
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUser(string id)
        {
            IdentityUser user = await _userMgr.FindByIdAsync(id);
            if (user != null)
            {
                IList<string> roles = await _userMgr.GetRolesAsync(user);
                UserViewModel userModel = new UserViewModel
                {
                    UserName = user.UserName,
                    Role = roles.First(),
                    IdentityId = id
                };
                return View(userModel);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PerformEditUser(UserViewModel userModel)
        {
            if (String.IsNullOrEmpty(userModel.Password) && String.IsNullOrEmpty(userModel.CofirmPassword))
            {
                //in case the user didn't touch password inputs we don't change password and hence don't need it's validation
                ModelState.Remove(nameof(UserViewModel.Password));
            }

            if (!String.IsNullOrEmpty(userModel.Password))
            {
                if (String.IsNullOrEmpty(userModel.CofirmPassword) || !userModel.CofirmPassword.Equals(userModel.Password))
                {
                    ModelState.AddModelError(nameof(UserViewModel.CofirmPassword), "Oba hasła muszą mieć tę samą wartość");
                }

                if (!IsPasswordValid(userModel.Password))
                {
                    ModelState.AddModelError(nameof(UserViewModel.Password), "Hasło powinno zawierać przynajmniej jedną wielką literę, jedną małą literę, jedną cyfrę i jeden znak specjalny: _ @ # $ % ^ & *");
                }
            }

            if (ModelState.IsValid)
            { 
                bool succeeded = true;
                IdentityUser user = await _userMgr.FindByIdAsync(userModel.IdentityId);

                string currentRole = (await _userMgr.GetRolesAsync(user)).First();
                bool changeRole = !userModel.Role.Equals(currentRole);
                if (changeRole && currentRole.Equals("admin") && (await GetAdminRolesCount()) == 1)
                {
                    succeeded = false;
                    ModelState.AddModelError("", "Nie można zmienić roli z 'admin' na inną - użytkownik jest ostatnim administratorem!");
                }

                if (succeeded && user.UserName != userModel.UserName)
                {
                    if (await DoesUserExist(userModel.UserName))
                    {
                        ModelState.AddModelError(nameof(UserViewModel.UserName), $"W serwisie już istnieje użytkownik '{userModel.UserName}'");
                        succeeded = false;
                    }
                    else
                    { 
                        //regexp used in UserModel class ensures us that login is valid
                        user.UserName = userModel.UserName;
                        await _userMgr.UpdateAsync(user);
                    }
                }

                if (succeeded)
                { 
                    if (!String.IsNullOrEmpty(userModel.Password))
                    {
                        //password is valid, as checked by IsPasswordValid private method, and equal to ConfirmPassword, as checked earlier in the method
                        await _userMgr.RemovePasswordAsync(user);
                        await _userMgr.AddPasswordAsync(user, userModel.Password);
                    }

                    if (changeRole)
                    {
                        await _userMgr.RemoveFromRoleAsync(user, currentRole);
                        await _userMgr.AddToRoleAsync(user, userModel.Role);
                    }

                    return RedirectToAction("ShowUsers");
                }
            }
            return View("EditUser", userModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            IdentityUser user = await _userMgr.FindByIdAsync(userId);
            string role = (await _userMgr.GetRolesAsync(user)).First();

            if (user != null && ((!role.Equals("admin")) || (await GetAdminRolesCount() > 1)))
            {
                await _userMgr.DeleteAsync(user);
                return RedirectToAction("ShowUsers");
            }
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        private bool IsPasswordValid(string passwordToCheck)
        {
            bool output = Regex.IsMatch(passwordToCheck, @"[A-Z]+");
            output &= Regex.IsMatch(passwordToCheck, @"[a-z]+");
            output &= Regex.IsMatch(passwordToCheck, @"[0-9]+");
            output &= Regex.IsMatch(passwordToCheck, @"[_@#$%^&*]+");

            return output;
        }

        private async Task<bool> DoesUserExist(string userName)
        {
            IdentityUser user = await _userMgr.FindByNameAsync(userName);
            if (user != null)
            {
                return true;
            }
            return false;
        }

        private async Task<int> GetAdminRolesCount()
        {
            int count = 0;

            foreach (IdentityUser user in _userMgr.Users)
            {
                if (await _userMgr.IsInRoleAsync(user, "admin"))
                {
                    count++;
                }
            }

            return count;
        }

        [HttpGet("/api/admincount")]
        public async Task<IActionResult> CountAdminRoles()
        {
            return Json(new { count = await GetAdminRolesCount() });
        }
    }
}
