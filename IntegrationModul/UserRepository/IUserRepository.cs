using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using IntegrationModul.Models;
using IntegrationModul.TokenModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IntegrationModul.UserRepository
{
    public interface IUserRepository
    {
        IntegrationModul.Models.User AddUser(UserRegisterRequest request);
        void ValidateEmail(ValidateEmailRequest request);
        Tokens JWTTokens(TokenRequest request);
    }
    public class UserRepository : IUserRepository 
    {
        private readonly IConfiguration _configuration;
        private readonly RwaMoviesContext _cbContext;
        private readonly IMapper _mapper;
        public UserRepository(RwaMoviesContext cbContext, IConfiguration configuration, IMapper mapper)
        {
            _cbContext = cbContext;
            _configuration = configuration;
            _mapper = mapper;
        }

        public IntegrationModul.Models.User AddUser(UserRegisterRequest request)
        {
            var usernamee = request.Username.ToLower().Trim();
            if (_cbContext.Users.Any(x => x.Username.Equals(usernamee))) 
            {
                throw new InvalidOperationException("That username already exists");
            }
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string b64Salt = Convert.ToBase64String(salt);
            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: request.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);

            string b64hash = Convert.ToBase64String(hash);
            byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
            string b64SecToken = Convert.ToBase64String(securityToken);

            var newUser = new IntegrationModul.Models.User
            {
                Username = request.Username,
                CreatedAt = DateTime.Now,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                CountryOfResidenceId = request.CountryOfResidenceId,
                IsConfirmed = false,
                SecurityToken = b64SecToken,
                PwdSalt = b64Salt,
                PwdHash = b64hash,
            };
            var blUser = _mapper.Map<BLUser>(newUser);
            var dbUser = _mapper.Map<BusinessLayer.DALModels.User>(blUser);
            _cbContext.Add(dbUser);
            CreateNotification(newUser);
            _cbContext.SaveChanges();
            return newUser;
        }

        private void CreateNotification(Models.User newUser)
        {
            var notification = new IntegrationModul.Models.Notification
            {
                CreatedAt = DateTime.Now,
                ReceiverEmail = newUser.Email,
                Subject = "Confirm your email adress!",
                Body = newUser.SecurityToken,
                SentAt = DateTime.Now,
            };
            var blNotification = _mapper.Map<BLNotification>(notification);
            var dbNotification = _mapper.Map<BusinessLayer.DALModels.Notification>(blNotification);
            _cbContext.Notifications.Add(dbNotification);
        }
        public Tokens JWTTokens(TokenRequest request)
        {
            var isaute = Authenticate(request.Username, request.Password);
            if (!isaute)
            {
                throw new InvalidOperationException("Authentication failed");
            }
            var key = _configuration["JWT:Key"];
            var keybytes = Encoding.UTF8.GetBytes(key);
            var TokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(ClaimTypes.Name, request.Username),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, request.Username)
                }),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(keybytes),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var tokendhandler = new JwtSecurityTokenHandler();
            var token = tokendhandler.CreateToken(TokenDescription);
            var seritoken = tokendhandler.WriteToken(token);
            return new Tokens
            {
                Token = seritoken
            };
        }

        private bool Authenticate(string username, string password)
        {
            var target = _cbContext.Users.Single(x => x.Username == username);
            if (target == null)
            {
                return false;
            }
            byte[] salt = Convert.FromBase64String(target.PwdSalt);
            byte[] hash = Convert.FromBase64String(target.PwdHash);
            byte[] calcHash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            return hash.SequenceEqual(calcHash);
        }

        public void ValidateEmail(ValidateEmailRequest request)
        {
            var target = _cbContext.Users.FirstOrDefault(x =>x.Username == request.Username && x.SecurityToken == request.B64SecToken);
            if (target == null)
            {
                throw new InvalidOperationException("User not found");
            }
            target.IsConfirmed = true;
            _cbContext.SaveChanges();
        }
    }
}
