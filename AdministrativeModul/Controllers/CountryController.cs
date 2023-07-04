using AdministrativeModul.ViewModels;
using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AdministrativeModul.Controllers
{
    public class CountryController : Controller
    {
        private readonly ILogger<CountryController> _logger;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ILogger<CountryController> logger, ICountryRepository countryRepository, IMapper mapper)
        {
            _logger = logger;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
        public IActionResult Country(int page, int size, string orderBy, string direction)
        {
            try
            {
                size = 10;

                var blCountries = _countryRepository.GetPageData(page, size, orderBy, direction);
                var vmCountries = _mapper.Map<IEnumerable<VMCountry>>(blCountries);
                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;

                ViewData["pages"] = _countryRepository.CountCoutnry() / size;
                return View(vmCountries);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public IActionResult CountryTable(int page, int size, string orderBy, string direction)
        {
            try
            {
                size = 10;
                var blCountries = _countryRepository.GetPageData(page, size, orderBy, direction);
                var vmCountries = _mapper.Map<IEnumerable<VMCountry>>(blCountries);
                return PartialView("_CountryTable", vmCountries);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public IActionResult Details(int id)
        {
            var blCountry = _countryRepository.GetCountryByID(id);
            if (blCountry == null)
            {
                return NotFound();
            }
            var country = _mapper.Map<VMCountry>(blCountry);
            return View(country);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(VMCountry country)
        {
            try
            {
                var blCountry = _mapper.Map<BLCountry>(country);
                var newcountry = _countryRepository.AddCountry(blCountry);
                var countryy = _mapper.Map<VMCountry>(newcountry);
                return RedirectToAction(nameof(Country));
            }
            catch(Exception)
            {
                return View();
            }
        }

        public IActionResult Edit(int id)
        {
            var blCountry = _countryRepository.GetCountryByID((int)id);
            if (blCountry ==  null)
            {
                return NotFound();
            }
            var vmCountry = _mapper.Map<VMCountry>(blCountry);
            return View(vmCountry);
        }
        [HttpPost]
        public IActionResult Edit(int id, VMCountry country)
        {
            try
            {
                var blCountry = _mapper.Map<BLCountry>(country);
                _countryRepository.UpdateCountry(id, blCountry);
                return RedirectToAction(nameof(Country));
            }
            catch
            {
                return View(country);
            }
        }
        public ActionResult Delete(int id)
        {
            var blCountry = _countryRepository.GetCountryByID(id);
            if (blCountry == null) 
            {
                return NotFound();
            }
            var deletedCountry = new VMCountry
            {
                Id = blCountry.Id,
                Code = blCountry.Code,
                Name = blCountry.Name
            };
            return View(deletedCountry);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VMCountry deleteCountry)
        {
            try
            {
                var country = _countryRepository.GetCountryByID(id);
                if (country == null)
                {
                    return NotFound();
                }
                var blcountry = _countryRepository.DeleteCountry(country);
                return RedirectToAction(nameof(Country));
            }
            catch
            {
                return RedirectToAction(nameof(Country));
            }
        }
    }
}
