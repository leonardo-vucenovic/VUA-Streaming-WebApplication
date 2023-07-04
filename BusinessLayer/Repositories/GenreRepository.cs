using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repositories
{
    public interface IGenreRepository
    {
        IEnumerable<BLGenre> GetAllGenres();
        BLGenre GetGenreByID(int id);
        BLGenre AddGenre(BLGenre genre);
        BLGenre UpdateGenre(int id,BLGenre genre);
        BLGenre DeleteGenre(BLGenre genre);
        IEnumerable<BLGenre> GetPageData(int page, int size, string orderBy, string direction);
        int CountGenre();
    }

    public class GenreRepository : IGenreRepository 
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public GenreRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public BLGenre AddGenre(BLGenre genre)
        {
            var newDbGenre = _mapper.Map<Genre>(genre);
            newDbGenre.Id = 0;
            _dbContext.Genres.Add(newDbGenre);
            _dbContext.SaveChanges();
            var newBlGenre = _mapper.Map<BLGenre>(newDbGenre);
            return newBlGenre;
        }
        public BLGenre UpdateGenre(int id, BLGenre genre)
        {
            var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
            if (dbGenre == null)
            {
                throw new InvalidOperationException("Genre not found");
            }
            _mapper.Map(genre, dbGenre);
            _dbContext.SaveChanges();
            var updatedBlGenre = _mapper.Map<BLGenre>(dbGenre);
            return updatedBlGenre;
        }
        public BLGenre DeleteGenre(BLGenre genre)
        {
            using(var databaseTransaction = _dbContext.Database.BeginTransaction()) 
            {
                try
                {
                    var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == genre.Id);
                    if (dbGenre == null)
                    {
                        throw new InvalidOperationException("Genre not found");
                    }
                    var dbVideos = _dbContext.Videos.Where(x => x.GenreId == genre.Id);
                    _dbContext.Videos.RemoveRange(dbVideos);
                    _dbContext.SaveChanges();
                    _dbContext.Genres.Remove(dbGenre);
                    _dbContext.SaveChanges();
                    databaseTransaction.Commit();
                    return _mapper.Map<BLGenre>(genre);
                }
                catch (Exception)
                {
                    databaseTransaction.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<BLGenre> GetAllGenres()
        {
            var dbGenres = _dbContext.Genres;
            var blGenres = _mapper.Map<IEnumerable<BLGenre>>(dbGenres);
            return blGenres;
        }

        public BLGenre GetGenreByID(int id)
        {
            var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
            var blGenre = _mapper.Map<BLGenre>(dbGenre);
            return blGenre;
        }

        public int CountGenre()
        {
            return _dbContext.Genres.Count();
        }

        public IEnumerable<BLGenre> GetPageData(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Genre> dbgenres = _dbContext.Genres.AsEnumerable();
            if (string.Compare(orderBy, "id", true) == 0)
            {
                dbgenres = dbgenres.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", true) == 0)
            {
                dbgenres = dbgenres.OrderBy(x => x.Name);
            }
            else if (string.Compare(orderBy, "description", true) == 0)
            {
                dbgenres = dbgenres.OrderBy(x => x.Description);
            }
            else
            {
                dbgenres = dbgenres.OrderBy(x => x.Id);
            }
            if (string.Compare(direction, "desc", true) == 0)
            {
                dbgenres = dbgenres.Reverse();
            }
            dbgenres = dbgenres.Skip(page * size).Take(size);
            var blgenres = _mapper.Map<IEnumerable<BLGenre>>(dbgenres);
            return blgenres;
        }
    }
}
