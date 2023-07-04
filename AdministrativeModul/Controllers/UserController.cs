using AdministrativeModul.ViewModels;
using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Linq;

namespace AdministrativeModul.Controllers
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
        public IActionResult User(string firstname, string lastname, string username, int countryID)
        {
            var blusers = _userRepositoryy.GetAllUser();
            if (!string.IsNullOrEmpty(Request.Cookies["firstname"]))
            {
                firstname = Request.Cookies["firstname"];
            }
            if (!string.IsNullOrEmpty(Request.Cookies["lastname"]))
            {
                lastname = Request.Cookies["lastname"];
            }
            if (!string.IsNullOrEmpty(Request.Cookies["username"]))
            {
                username = Request.Cookies["username"];
            }
            if (!string.IsNullOrEmpty(Request.Cookies["countryID"]))
            {
                int.TryParse(Request.Cookies["countryID"], out countryID);
            }
            if (!string.IsNullOrEmpty(firstname))
            {
                blusers = blusers.Where(x => x.FirstName.Contains(firstname));
            }
            if (!string.IsNullOrEmpty(lastname))
            {
                blusers = blusers.Where(x => x.LastName.Contains(lastname));
            }
            if (!string.IsNullOrEmpty(username))
            {
                blusers = blusers.Where(x => x.Username.Contains(username));
            }
            if (countryID > 0)
            {
                blusers = blusers.Where(x => x.CountryOfResidenceId == countryID);
            }

            var vmUsers = _mapper.Map<IEnumerable<VMUser>>(blusers);
            var countryIDS = vmUsers.Select(x => x.CountryOfResidenceId).Distinct();
            var countryNAMES = _countryRepository.GetAllCountrys().Where(x => countryIDS.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Name);
            foreach (var user in vmUsers)
            {
                if (countryNAMES.TryGetValue(user.CountryOfResidenceId, out string name))
                {
                    if (user.CountryOfResidence == null)
                    {
                        user.CountryOfResidence = new VMCountry();
                    }
                }
            }
            var blCountry = _countryRepository.GetAllCountrys();
            var vmCountry = _mapper.Map<IEnumerable<VMCountry>>(blCountry);
            ViewBag.Country = new SelectList(vmCountry, "Id", "Name");
            return View(vmUsers);
        }
        public IActionResult Details(int id)
        {
            var blUser = _userRepositoryy.GetUserById(id);
            if (blUser == null)
            {
                return NotFound();
            }
            var vmUser = _mapper.Map<VMUser>(blUser);
            var country = _countryRepository.GetCountryByID(vmUser.CountryOfResidenceId);
            ViewBag.Country = country?.Name;
            return View(vmUser);
        }
        public IActionResult Create()
        {
            var blCountry = _countryRepository.GetAllCountrys();
            var vmCountry = _mapper.Map<IEnumerable<VMCountry>>(blCountry);
            ViewBag.Country = new SelectList(vmCountry, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(VMUserRegister userregister) 
        {
            if (!ModelState.IsValid) 
            {
                return View(userregister);
            }
            try
            {
                var user = _userRepositoryy.CreateUser(
                       userregister.Username,
                       userregister.FirstName,
                       userregister.LastName,
                       userregister.Email,
                       userregister.Phone,
                       userregister.Password2,
                       userregister.CountryOfResidenceId);
                return RedirectToAction("User");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "User with that email already exists!.");
            }
        }
        public ActionResult Delete(int id)
        {
            var user = _userRepositoryy.GetUserById(id);
            var deletedUser = new VMUser
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
            return View(deletedUser);
        }
        [HttpPost]
        public IActionResult Delete(int id, VMUser deletedUser) 
        {
            var user = _userRepositoryy.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            _userRepositoryy.DeleteUser(user.Id);
            return RedirectToAction(nameof(User));
        }
        public IActionResult  Edit(int id)
        {
            var blUser = _userRepositoryy.GetUserById(id);
            if (blUser == null)
            {
                return NotFound();
            }
            var vmUser = _mapper.Map<VMUser>(blUser);
            var blCountry = _countryRepository.GetAllCountrys();
            var vmCountry = _mapper.Map<IEnumerable<VMCountry>>(blCountry);
            ViewBag.Country = new SelectList(vmCountry, "Id", "Name", vmUser.CountryOfResidenceId);
            return View(vmUser);
        }

        [HttpPost]
        public IActionResult Edit(VMUser user)
        {
            var blUser = _mapper.Map<BLUser>(user);
            _userRepositoryy.UpdateUser(blUser.Id, blUser);
            return RedirectToAction(nameof(User));
        }
    }
}
