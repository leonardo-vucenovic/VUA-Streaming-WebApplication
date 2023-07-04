using AdministrativeModul.ViewModels;
using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdministrativeModul.Controllers
{
    public class GenreController : Controller
    {
        private readonly ILogger<CountryController> _logger;
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;
        public GenreController(ILogger<CountryController> logger, IGenreRepository genreRepository, IMapper mapper)
        {
            _logger = logger;
            _genreRepository = genreRepository;
            _mapper = mapper;
        }
        public IActionResult Genre(int page, int size, string orderBy, string direction)
        {
            try
            {
                size = 10;
                var blgenres = _genreRepository.GetPageData(page, size, orderBy, direction);
                var vmgenres = _mapper.Map<IEnumerable<VMGenre>>(blgenres);
                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = _genreRepository.CountGenre() / size;
                return View(vmgenres);
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
                var blgenres = _genreRepository.GetPageData(page, size, orderBy, direction);
                var vmgenres = _mapper.Map<IEnumerable<BLGenre>>(blgenres);
                return PartialView("_GenreTable", vmgenres);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public IActionResult Details(int id)
        {
            var blgenres = _genreRepository.GetGenreByID(id);
            if (blgenres == null)
            {
                return NotFound();
            }
            var genre = _mapper.Map<VMGenre>(blgenres);
            return View(genre);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(VMGenre genre)
        {
            try
            {
                var blgenre = _mapper.Map<BLGenre>(genre);
                var newgenre = _genreRepository.AddGenre(blgenre);
                var genreee = _mapper.Map<VMGenre>(newgenre);
                return RedirectToAction(nameof(Genre));
            }
            catch (Exception)
            {
                return View();
            }
        }
        public IActionResult Edit(int id)
        {
            var blgenre = _genreRepository.GetGenreByID((int)id);
            if (blgenre == null)
            {
                return NotFound();
            }
            var vmgenre = _mapper.Map<VMGenre>(blgenre);
            return View(vmgenre);
        }
        [HttpPost]
        public IActionResult Edit(int id, VMGenre genre)
        {
            try
            {
                var blgenre = _mapper.Map<BLGenre>(genre);
                _genreRepository.UpdateGenre(id, blgenre);
                return RedirectToAction(nameof(Genre));
            }
            catch
            {
                return View(genre);
            }
        }
        public ActionResult Delete(int id)
        {
            var blgenre = _genreRepository.GetGenreByID(id);
            if (blgenre == null)
            {
                return NotFound();
            }
            var deletedGenre = new VMGenre
            {
                Id = blgenre.Id,
                Name = blgenre.Name,
                Description = blgenre.Description
            };
            return View(deletedGenre);
        }
        [HttpPost]
        public ActionResult Delete(int id, VMGenre vmgenre)
        {
            try
            {
                var genre = _genreRepository.GetGenreByID(id);
                if (genre == null)
                {
                    return NotFound();
                }
                var blgenre = _genreRepository.DeleteGenre(genre);
                return RedirectToAction(nameof(Genre));
            }
            catch
            {
                return RedirectToAction(nameof(Genre));
            }
        }
    }
}
