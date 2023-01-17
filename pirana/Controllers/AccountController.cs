using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pirana.Models;
using pirana.Utilies.Roles;
using pirana.ViewModels;

namespace pirana.Controllers
{
    public class AccountController : Controller
    {
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVm registerVm)
        {
            if(!ModelState.IsValid) return View();
            AppUser user = new AppUser { Email=registerVm.Email,FirstName=registerVm.Name,LastName=registerVm.Surname,UserName=registerVm.Username};
            IdentityResult result = await _userManager.CreateAsync(user,registerVm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Index","Home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public  async Task<IActionResult> Login(UserLoginVm loginVm,string? ReturnUrl )
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByEmailAsync(loginVm.UsernameorEmail);
            if (user == null)
            {
                user=await _userManager.FindByNameAsync(loginVm.UsernameorEmail);
                if(user == null)
                {
                    ModelState.AddModelError("","Your username or password is wrong");
                    return View();
                }
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVm.Password, loginVm.IsPersistance, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Your username or password is wrong");
                return View();
            }
            if (ReturnUrl ==null) 
            {
                return RedirectToAction("Index","Home"); 
            }
            else 
            {
                return Redirect(ReturnUrl); 
            }
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }
        //public async Task<IActionResult> AddRole()
        //{
        //    foreach (var item in Enum.GetValues(typeof(Roles)))
        //    {
        //      await _roleManager.CreateAsync(new IdentityRole { Name =item.ToString()});
        //    }
        //    return View();
        //}

        //public async Task<IActionResult> Addroleuser()
        //{
        //    var user = await _userManager.FindByNameAsync("phatehh");
        //    await _userManager.AddToRoleAsync(user,Roles.Admin.ToString());
        //    user = await _userManager.FindByNameAsync("Admin");
        //    await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
        //    return View();
        //}
    }
}
