using Azure.Identity;
using BusinessLayer.Repositories;
using IntegrationModul.Models;
using IntegrationModul.TokenModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationModul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository.IUserRepository _userRepository;

        public UserController(UserRepository.IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("[action]")]
        public ActionResult<User> Register([FromQuery] string username, [FromQuery] string firstname, [FromQuery] string lastname, [FromQuery] string password, [FromQuery] string confirmPassword, [FromQuery] string email, [FromQuery] string phone, [FromQuery] int countryOfResidenceId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                UserRegisterRequest request = new UserRegisterRequest
                {
                    Username = username,
                    FirstName = firstname,
                    LastName = lastname,
                    Password = password,
                    ConfirmPassword = confirmPassword,
                    Email = email,
                    Phone = phone,
                    CountryOfResidenceId = countryOfResidenceId
                };
                var newUser = _userRepository.AddUser(request);
                return Ok(new UserRegisterResponse
                {
                    Id = newUser.Id,
                    SecurityToken = newUser.SecurityToken
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with register the user. Country ID does not exists!");
            }
        }
        [HttpPost("[action]")]
        public ActionResult ValidateEmail([FromQuery] string username, [FromQuery] string b64SecToken)
        {
            try
            {
                if (!ModelState.IsValid)
                { 
                    return BadRequest(ModelState); 
                }
                ValidateEmailRequest request = new ValidateEmailRequest
                {
                    Username = username,
                    B64SecToken = b64SecToken
                };
                _userRepository.ValidateEmail(request);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with validate the email.");
            }
        }
        [HttpPost("[action]")]
        public ActionResult<Tokens> JwtTokens([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                TokenRequest request = new TokenRequest
                {
                    Username = username,
                    Password = password
                };
                return Ok(_userRepository.JWTTokens(request));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
