using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using IntegrationModul.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Runtime.CompilerServices;

namespace IntegrationModul.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreController(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }
        [HttpPost("[action]")]
        public ActionResult<Genre> CreateGenre([FromQuery] string name, [FromQuery] string description) 
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("Name is required");
                }
                var genre = new Models.Genre
                {
                    Name = name,
                    Description = description
                };
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var blGenre = _mapper.Map<BLGenre>(genre);
                var newGenre = _genreRepository.AddGenre(blGenre);
                var createdGenre = _mapper.Map<Genre>(newGenre);
                return Ok(createdGenre);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with creating the genre");
            }
        }
        [HttpPut("[action]/{id}")]
        public ActionResult<Genre> UpdateGenre(int id, [FromQuery] string name, [FromQuery] string description)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var blGenre = _genreRepository.GetGenreByID(id);
                if (blGenre == null)
                {
                    return NotFound($"Genre for update with ID: {id} is not found");
                }
                blGenre.Name = name;
                blGenre.Description = description;
                var updatedGenre = _genreRepository.UpdateGenre(id, blGenre);
                var genree = _mapper.Map<Genre>(updatedGenre);
                return Ok(genree);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with updating the genre");
            }
        }
        [HttpDelete("[action]/{id}")]
        public ActionResult<Genre> DeleteGenre(int id) 
        {
            try
            {
                var blgenre = _genreRepository.GetGenreByID(id);
                if (blgenre == null)
                {
                    return NotFound($"Genre with ID: {id} is not found");
                }
                var blgenredelete = _genreRepository.DeleteGenre(blgenre);
                return Ok(blgenredelete);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with deleting the genre, genre with that id is connect with other table");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Genre>> GetAllGenres() 
        {
            try
            {
                var blGenres = _genreRepository.GetAllGenres();
                if (blGenres == null)
                {
                    return NotFound($"There are no genres in the database");
                }
                var genres = _mapper.Map<IEnumerable<Genre>>(blGenres);
                return Ok(genres);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching all genres");
            }
        }
        [HttpGet("[action]/{id}")]
        public ActionResult<Genre> GetGenreByID(int id) 
        {
            try
            {
                var blgenre = _genreRepository.GetGenreByID(id);
                if (blgenre == null)
                {
                    return NotFound($"Genre with ID: {id} is not found");
                }
                var genre = _mapper.Map<Genre>(blgenre);
                return Ok(genre);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching genre");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Genre>> SearchGenresBySpecificPartOfName(string part)
        {
            try
            {
                var blGenres = _genreRepository.GetAllGenres().Where(x => x.Name.ToLower().Contains(part.ToLower()));
                if (blGenres == null)
                {
                    return NotFound($"Genres with that part is not found");
                }
                var genres = _mapper.Map<IEnumerable<Genre>>(blGenres);
                return Ok(genres);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching genres");
            }
        }
    }
}
