using AutoMapper;
using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PublicModul.ViewModels;
using System.Drawing;
using System.Security.Claims;

namespace PublicModul.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepositoryy _userRepositoryy;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger, IUserRepositoryy userRepositoryy, ICountryRepository countryRepository, IMapper mapper)
        {
            _userRepositoryy = userRepositoryy;
            _countryRepository = countryRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Register()
        {
            var blCountry = _countryRepository.GetAllCountrys();
            var vmCountry = _mapper.Map<IEnumerable<VMCountry>>(blCountry);
            ViewBag.Country = new SelectList(vmCountry, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Register(VMRegister register)
        {
            if (!ModelState.IsValid)
            {
                var blCountry = _countryRepository.GetAllCountrys();
                var vmCountry = _mapper.Map<IEnumerable<VMCountry>>(blCountry);
                ViewBag.Country = new SelectList(vmCountry, "Id", "Name");
                return View(register);
            }
            try
            {
                var user = _userRepositoryy.CreateUser
                    (
                        register.Username,
                        register.FirstName,
                        register.LastName,
                        register.Email,
                        register.Phone,
                        register.Password,
                        register.CountryOfResidenceId
                    );
            }
            catch (Exception)
            {
                var blCountry = _countryRepository.GetAllCountrys();
                var vmCountry = _mapper.Map<IEnumerable<VMCountry>>(blCountry);
                ViewBag.Country = new SelectList(vmCountry, "Id", "Name");
                return View(register);
            }
            return RedirectToAction("Home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(VMLogin login, bool staySignedIn)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            try
            {
                var user = _userRepositoryy.GetConfirmedUser(login.Username, login.Password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    return View(login);
                }
                var claim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                };
                var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                var autentificationProperitie = new AuthenticationProperties();
                if (staySignedIn)
                {
                    autentificationProperitie.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(365);
                }

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), autentificationProperitie).Wait();

                return RedirectToAction("Video", "Video");
            }
            catch (Exception)
            {
                return View(login);
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Home");
        }
        public IActionResult Details(string username)
        {
            var blUser = _userRepositoryy.GetUserByName(username);
            if (username == null)
            {
                return NotFound();
            }
            var vmUser = _mapper.Map<VMUser>(blUser);
            var country = _countryRepository.GetCountryByID(vmUser.Id);
            ViewBag.CountryName = country?.Name;
            return View(vmUser);
        }
        public IActionResult ValidateEmail(VMValidateEmail validateEmail)
        {
            if (!ModelState.IsValid)
            {
                return View(validateEmail);
            }
            _userRepositoryy.ConfirmEmail(validateEmail.Email, validateEmail.SecurityToken);
            return RedirectToAction("Video", "Video");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(VMChangePassword userForChange)
        {
            _userRepositoryy.ChangePassword(userForChange.Username, userForChange.Password);
            return RedirectToAction("Video", "Video");
        }
    }
}
