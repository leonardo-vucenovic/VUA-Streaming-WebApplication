using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using IntegrationModul.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationModul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
        
        [HttpPost("[action]")]
        public ActionResult<Country> CreateCounutry([FromQuery] string name, [FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(code))
                {
                    return BadRequest("Name and code is required");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Country country = new Country
                {
                    Name = name,
                    Code = code
                };
                var blCounutry = _mapper.Map<BLCountry>(country);
                var newCounutry = _countryRepository.AddCountry(blCounutry);
                var counutryy = _mapper.Map<Country>(newCounutry);
                return Ok(counutryy);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with creating the counutry.");
            }
        }
        
        [HttpPut("[action]/{id}")]
        public ActionResult<Country> UpdateCountry(int id, [FromQuery] string name, [FromQuery] string code)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var blcounutry = _countryRepository.GetCountryByID(id);
                if (blcounutry == null)
                {
                    return NotFound($"Counutry for update with ID: {id} is not found");
                }
                blcounutry.Name = name;
                blcounutry.Code = code;
                var updatedCounutry = _countryRepository.UpdateCountry(id, blcounutry);
                var countryy = _mapper.Map<Country>(updatedCounutry);
                return Ok(countryy);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with updating the counutry.");
            }
        }
        
        [HttpDelete("[action]/{id}")]
        public ActionResult DeleteCountry(int id) 
        {
            try
            {
                var blCounutry = _countryRepository.GetCountryByID(id);
                if (blCounutry == null)
                {
                    return NotFound($"Country with ID: {id} is not found");
                }
                var blcountrydeleted = _countryRepository.DeleteCountry(blCounutry);
                return Ok(blcountrydeleted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with deleting the counutry, country with that id is connect with other table");
            }
        }
        
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Country>> GetAllCountries() 
        {
            try
            {
                var blCounutries = _countryRepository.GetAllCountrys();
                if (blCounutries == null)
                {
                    return NotFound($"There are no countries in the database.");
                }
                var countries = _mapper.Map<IEnumerable<Country>>(blCounutries);
                return Ok(countries);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching all countries.");
            }
        }
        
        [HttpGet("[action]")]
        public ActionResult<Country> GetCountryByID(int id) 
        {
            try
            {
                var blCountry = _countryRepository.GetCountryByID(id);
                if (blCountry == null)
                {
                    return NotFound($"Country with ID: {id} is not found.");
                }
                var counutryy = _mapper.Map<Country>(blCountry);
                return Ok(counutryy);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching all country.");
            }
        }
        
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Country>> SearchCounutryBySpecificPartOfName(string part)
        {
            try
            {
                var blCounutriey = _countryRepository.GetAllCountrys().Where(x => x.Name.ToLower().Contains(part.ToLower()));
                if (blCounutriey == null)
                {
                    return NotFound($"Counutry with that part is not found.");
                }
                var counutries = _mapper.Map<IEnumerable<Country>>(blCounutriey);
                return Ok(counutries);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching country.");
            }
        }
        
        
    }
}
