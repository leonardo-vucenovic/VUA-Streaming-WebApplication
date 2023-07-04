using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repositories
{
    public interface ICountryRepository
    {
        IEnumerable<BLCountry> GetAllCountrys();
        BLCountry GetCountryByID(int id);
        BLCountry AddCountry(BLCountry country);
        BLCountry UpdateCountry(int id, BLCountry country);
        BLCountry DeleteCountry(BLCountry country);
        int CountCoutnry();
        IEnumerable<BLCountry> GetPageData(int page, int size, string orderBy, string direction);
    }
    public class CountryRepository : ICountryRepository
    {
        private readonly RwaMoviesContext _dbcontext;
        private readonly IMapper _mapper;
        public CountryRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbcontext = dbContext;
            _mapper = mapper;
        }

        public BLCountry AddCountry(BLCountry country)
        {
            var newDbCountry = _mapper.Map<Country>(country);
            newDbCountry.Id = 0;
            _dbcontext.Countries.Add(newDbCountry);
            _dbcontext.SaveChanges();
            var newBlCoutnry = _mapper.Map<BLCountry>(country);
            return newBlCoutnry;
        }

        public BLCountry UpdateCountry(int id, BLCountry country)
        {
            var dbCountry = _dbcontext.Countries.FirstOrDefault(x => x.Id == id);
            if (dbCountry == null)
            {
                throw new InvalidOperationException("Country not found");
            }
            _mapper.Map(country, dbCountry);
            _dbcontext.SaveChanges();
            var updatedBlCountry = _mapper.Map<BLCountry>(country);
            return updatedBlCountry;
        }

        public BLCountry DeleteCountry(BLCountry country)
        {
            using (var databaseTransaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var dbcountry = _dbcontext.Countries.FirstOrDefault(x => x.Id == country.Id);
                    if (dbcountry == null)
                    {
                        throw new InvalidOperationException("Country not found");
                    }
                    var dbUsers = _dbcontext.Users.Where(x => x.CountryOfResidenceId == dbcountry.Id);
                    _dbcontext.Users.RemoveRange(dbUsers);
                    _dbcontext.SaveChanges();
                    _dbcontext.Countries.Remove(dbcountry);
                    _dbcontext.SaveChanges();
                    databaseTransaction.Commit();
                    return _mapper.Map<BLCountry>(country);
                }
                catch (Exception)
                {
                    databaseTransaction.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<BLCountry> GetAllCountrys()
        {
            var dbCountries = _dbcontext.Countries;
            var blCountries = _mapper.Map<IEnumerable<BLCountry>>(dbCountries);
            return blCountries;
        }

        public BLCountry GetCountryByID(int id)
        {
            var dbCountry = _dbcontext.Countries.FirstOrDefault(x => x.Id == id);
            var blCountry = _mapper.Map<BLCountry>(dbCountry);
            return blCountry;
        }

        public int CountCoutnry()
        {
            return _dbcontext.Countries.Count();
        }

        public IEnumerable<BLCountry> GetPageData(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Country> dbdountries = _dbcontext.Countries.AsEnumerable();
            if (string.Compare(orderBy, "id", true) == 0)
            {
                dbdountries = dbdountries.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "code", true) == 0)
            {
                dbdountries = dbdountries.OrderBy(x => x.Code);
            }
            else if (string.Compare(orderBy, "name", true) == 0)
            {
                dbdountries = dbdountries.OrderBy(x => x.Name);
            }
            else
            {
                dbdountries = dbdountries.OrderBy(x => x.Id);
            }
            if (string.Compare(direction, "desc", true) == 0)
            {
                dbdountries = dbdountries.Reverse();
            }
            dbdountries = dbdountries.Skip(page * size).Take(size);
            var blcountrys = _mapper.Map<IEnumerable<BLCountry>>(dbdountries);
            return blcountrys;
        }
    }
}
