using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BusinessLayer.Repositories
{
    public interface IUserRepositoryy
    {
        IEnumerable<BLUser> GetAllUser();
        BLUser GetUserById(int id);
        BLUser UpdateUser(int id, BLUser user);
        BLUser CreateUser(string username, string firstname, string lastname, string email, string phone, string password, int country);
        void DeleteUser(int id);
        void SoftDeleteUser(int id);
        IEnumerable<BLCountry> GetCountries();
        BLUser GetConfirmedUser(string username, string password);
        void ConfirmEmail(string email,string securityToken);
        void ChangePassword (string username, string password);
        BLUser GetUserByName(string username);
    }

    public class UserRepositoryy : IUserRepositoryy
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UserRepositoryy(RwaMoviesContext dbContext, IMapper mapper, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        public BLUser CreateUser(string username, string firstname, string lastname, string email, string phone, string password, int country)
        {
            if (_dbContext.Users.Any(x => x.Email == email))
            {
                throw new Exception("User with that email already exists.");
            }
            (var salt, var b64Salt) = GenerateSalt();
            var b64Hash = CreateHash(password, salt);
            var b64SecToken = GenerateSecurityToken();
            var dbUser = new User()
            {
                CreatedAt = DateTime.UtcNow,
                Username = username,
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                Phone = phone,
                PwdHash = b64Hash,
                PwdSalt = b64SecToken,
                SecurityToken = b64SecToken,
                CountryOfResidenceId = country
            };
            _dbContext.Users.Add(dbUser);
            _dbContext.SaveChanges();
            var blUser = _mapper.Map<BLUser>(dbUser);
            return blUser;
        }
        private static (byte[], string) GenerateSalt()
        {
            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var b64Salt = Convert.ToBase64String(salt);

            return (salt, b64Salt);
        }
        private static string GenerateSecurityToken()
        {
            byte[] securityToken = RandomNumberGenerator.GetBytes(256 / 8);
            string b64SecToken = Convert.ToBase64String(securityToken);

            return b64SecToken;
        }
        public void DeleteUser(int id)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (dbUser == null)
            {
                throw new InvalidOperationException("User not found");
            }
            _dbContext.Users.Remove(dbUser);
            _dbContext.SaveChanges();
        }

        public IEnumerable<BLUser> GetAllUser()
        {
            var dbUsers = _dbContext.Users;
            var blUsers = _mapper.Map<IEnumerable<BLUser>>(dbUsers);
            return blUsers;
        }

        public BLUser GetUserById(int id)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            var blUser = _mapper.Map<BLUser>(dbUser);
            return blUser;
        }

        public void SoftDeleteUser(int id)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (dbUser == null)
            {
                throw new InvalidOperationException("User not found");
            }
            dbUser.DeletedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
        }

        public BLUser UpdateUser(int id, BLUser user)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x => x.Id ==id);
            if (dbUser == null)
            {
                throw new InvalidOperationException("User not found");
            }
            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.Email = user.Email;
            dbUser.Phone = user.Phone;
            dbUser.CountryOfResidenceId = user.CountryOfResidenceId;
            _dbContext.SaveChanges();
            var blUser = _mapper.Map<BLUser>(dbUser);
            return blUser;
        }

        public IEnumerable<BLCountry> GetCountries()
        {
            var dbCountries = _dbContext.Countries.ToList();
            var blCountries = _mapper.Map<IEnumerable<BLCountry>>(dbCountries);
            return blCountries;
        }

        public BLUser GetConfirmedUser(string username, string password)
        {
            var dbUsers = _dbContext.Users.FirstOrDefault(x => x.Username == username);
            if (dbUsers == null)
            {
                throw new InvalidOperationException("Wrong username");
            }

            var salt = Convert.FromBase64String(dbUsers.PwdSalt);
            var b64hash = CreateHash(password, salt);

            if (dbUsers.PwdHash == b64hash )
            {
                throw new InvalidOperationException("Wrong password");
            }
            var blUser = _mapper.Map<BLUser>(dbUsers);
            return blUser;
        }
        private static string CreateHash(string password, byte[] salt)
        {
            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            return b64Hash;
        }
        public void ConfirmEmail(string email, string securityToken)
        {
            var userToConfirm = _dbContext.Users.FirstOrDefault(x => x.Email == email && x.SecurityToken == securityToken);
            userToConfirm.IsConfirmed = true;
            _dbContext.SaveChanges();
        }

        public void ChangePassword(string username, string password)
        {
            User userToChange = _dbContext.Users.FirstOrDefault(x => x.Username == username && !x.DeletedAt.HasValue);
            (var salt, var b64Salt) = GenerateSalt();
            var b65hash = CreateHash(password, salt);
            userToChange.PwdHash = b65hash;
            userToChange.PwdSalt = b64Salt;
            _dbContext.SaveChanges();
        }

        public BLUser GetUserByName(string username)
        {
            var nameDetails = username.Split(' ');
            var first = nameDetails[0];
            var last = nameDetails[1];

            var dbUser = _dbContext.Users.FirstOrDefault(x => x.FirstName ==  first && x.LastName == last);
            var blUser = _mapper.Map<BLUser>(dbUser);
            return blUser;
        }
    }
}
