using AdministrativeModul.ViewModels;
using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdministrativeModul.Controllers
{
    public class VideoController : Controller
    {
        private readonly ILogger<VideoController> _logger;
        private readonly IVideoRepository _videoRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly RwaMoviesContext _dbContext;

        public VideoController(ILogger<VideoController> logger, IVideoRepository videoRepository,
            IGenreRepository genreRepository, IImageRepository imageRepository,
            ITagRepository tagRepository, IMapper mapper, RwaMoviesContext dbContext)
        {
            _logger = logger;
            _videoRepository = videoRepository;
            _genreRepository = genreRepository;
            _imageRepository = imageRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public ActionResult Video(string filter, int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                {
                    size = 5;
                }
                var videos = _videoRepository.GetAllVideos();
                if (!string.IsNullOrEmpty(filter))
                {
                    videos = videos.Where(v => v.Name.Contains(filter));
                }
                var totalVideos = videos.Count();

                var totalPages = (int)Math.Ceiling((double)totalVideos / size);
                page = Math.Max(0, Math.Min(page, totalPages - 1));

                videos = videos.Skip(page * size).Take(size);
                var videoViewModels = videos.Select(v => new VMVideo
                {
                    Id = v.Id,
                    CreatedAt = v.CreatedAt,
                    Name = v.Name,
                    Description = v.Description,
                    GenreId = v.GenreId,
                    TotalSeconds = v.TotalSeconds,
                    StreamingUrl = v.StreamingUrl
                });
                var genreMappings = _videoRepository.GetGenres();
                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = totalPages;

                ViewBag.Genres = genreMappings;
                return View(videoViewModels);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public IActionResult VideoTableBodyPartial(string filter, int page, int size, string orderBy, string direction) 
        {
            try
            {
                if (size == 0)
                {
                    size = 5;
                }
                var videos = _videoRepository.GetAllVideos();
                if (!string.IsNullOrEmpty(filter))
                {
                    videos = videos.Where(v => v.Name.Contains(filter));
                }
                var blVideo = _videoRepository.GetPagedData(page, size, orderBy, direction);
                var vmVideo = _mapper.Map<IEnumerable<VMVideo>>(blVideo);

                var genreMappings = _videoRepository.GetGenres();
                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = _videoRepository.VideoCount() / size;

                ViewBag.Genres = genreMappings;
                return PartialView("_VideoTableBodyPartial",vmVideo);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public IActionResult Details(int id) 
        { 
            var blVideo = _videoRepository.GetVideoById(id);
            if (blVideo == null)
            {
                return NotFound();
            }
            var vmVideo = _mapper.Map<VMVideo>(blVideo);
            var genre = _genreRepository.GetGenreByID(vmVideo.GenreId);
            var image = _imageRepository.GetImageByID(vmVideo.ImageId);
            var videoTags = _videoRepository.GetVideoTagsByVideoId(id);
            var vmVideoTags = videoTags.ToList();
            ViewBag.GenreName = genre?.Name;
            ViewBag.ImageName = image?.Content; 
            ViewBag.VideoTags = vmVideoTags;
            return View(vmVideo);
        }
        public IActionResult Delete(int id) 
        {
            var blVideo = _videoRepository.GetVideoById(id);
            if (blVideo == null) 
            {
                return NotFound();
            }
            var vmVideo = _mapper.Map<VMVideo>(blVideo);
            return View(vmVideo);
        }
        [HttpPost]
        public IActionResult Delete(int id, VMVideo vmVideo)
        {
            try
            {
                var blVideo = _videoRepository.GetVideoById(id);
                if (blVideo == null)
                {
                    return NotFound();
                }
                _videoRepository.DeleteVideo(id);
                return RedirectToAction(nameof(Video));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Video));
            }
        }
        public IActionResult Create()
        {
            try
            {
                var genreOptions = _dbContext.Genres.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();
                var imageOptions = _dbContext.Images.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Content }).ToList();
                ViewBag.GenreOptions = genreOptions;
                ViewBag.ImageOptions = imageOptions;
                return View();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public IActionResult Create(VMVideoAdd addvideo) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var video = _videoRepository.AddVideoBL(
                    addvideo.Name,
                    addvideo.Description,
                    addvideo.GenreId,
                    addvideo.ImageId,
                    addvideo.TotalSeconds,
                    addvideo.StreamingUrl);
                return RedirectToAction("Video");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public ActionResult Edit(int id)
        {
            var video = _videoRepository.GetVideoById(id);
            if (video == null) 
            {
                return NotFound();
            }
            var editVideo = _mapper.Map<VMVideoEdit>(video);
            var genreOptions = _dbContext.Genres.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();
            var imageOptions = _dbContext.Images.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Content }).ToList();
            ViewBag.GenreOptions = genreOptions;
            ViewBag.ImageOptions = imageOptions;
            return View(editVideo);
        }
        [HttpPost]
        public ActionResult Edit(int id, VMVideoEdit videoforedit)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return NotFound();
                }
                var video = _videoRepository.GetVideoById(id);
                if (video == null)
                {
                    return NotFound();
                }
                video.Name = videoforedit.Name;
                video.Description = videoforedit.Description;
                video.TotalSeconds = videoforedit.TotalSeconds;
                video.StreamingUrl = videoforedit.StreamingUrl;
                video.GenreId = videoforedit.GenreId;
                video.ImageId = videoforedit.ImageId;
                var updatedvideo = _videoRepository.UpdateVideo(id,video);
                return RedirectToAction(nameof(Video));
            }
            catch (Exception)
            {
                return View(videoforedit);
            }
        }
     }
}
