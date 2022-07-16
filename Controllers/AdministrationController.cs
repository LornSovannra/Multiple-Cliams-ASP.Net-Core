using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Multi_Claims.Data;
using Multi_Claims.Models;
using Multi_Claims.ViewModels;
using System.Security.Claims;

namespace Multi_Claims.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();

            ViewBag.Users = users;
            return View();
        }

        public async Task<IActionResult> ManageUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);    

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id {userId} not found.";

                return RedirectToAction("NotFound");
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsVM
            {
                UserId = userId,
            };

            foreach(Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                };

                // If the user has the claim, set IsSelected property to true, so UI will check on checkbox
                if(existingUserClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUser(UserClaimsVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id {model.UserId} not found.";

                return RedirectToAction("NotFound");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Can't remove user claims");
                return View(model);
            }

            result = await _userManager.AddClaimsAsync(user,
                model.Cliams.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType)));

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Can't remove user claims");
                return View(model);
            }

            return RedirectToAction("ManageUser", new { userId = model.UserId });
        }

        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
