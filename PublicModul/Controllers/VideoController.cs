using AutoMapper;
using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using PublicModul.ViewModels;

namespace PublicModul.Controllers
{
    public class VideoController : Controller
    {
        private readonly ILogger<VideoController> _logger;
        private readonly IVideoRepository _videoRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public VideoController(ILogger<VideoController> logger, IVideoRepository videoRepository, IGenreRepository genreRepository, IImageRepository imageRepository, IMapper mapper)
        {
            _logger = logger;
            _videoRepository = videoRepository;
            _genreRepository = genreRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Video(int page, int size, string orderBy, string direction, string videoName)
        {
            if (size == 0)
            {
                size = 6;
            }
            var blVideo = _videoRepository.GetPagedData(page, size, orderBy, direction);
            var vmVideo = _mapper.Map<IEnumerable<VMVideo>>(blVideo);
            foreach (var video in vmVideo)
            {
                var blGenre = _genreRepository.GetGenreByID(video.GenreId);
                video.GenreName = blGenre?.Name;

                var blImage = _imageRepository.GetImageByID(video.ImageId);
                video.ImageContent = blImage?.Content;
            }
            ViewData["page"] = page;
            ViewData["size"] = size;
            ViewData["orderBy"] = orderBy;
            ViewData["direction"] = direction;
            ViewData["totalPages"] = _videoRepository.VideoCount() / size;
            return View(vmVideo);
        }
        public IActionResult VideoTableBodyPartial(int page, int size, string orderBy, string direction, string videoName) 
        {
            if (size == 0)
            {
                size = 6;
            }
            var blVideo = _videoRepository.GetPagedData(page, size, orderBy, direction);
            var vmVideo = _mapper.Map<IEnumerable<VMVideo>>(blVideo);
            foreach (var video in vmVideo)
            {
                var blGenre = _genreRepository.GetGenreByID(video.GenreId);
                video.GenreName = blGenre?.Name;

                var blImage = _imageRepository.GetImageByID(video.ImageId);
                video.ImageContent = blImage?.Content;
            }
            ViewData["page"] = page;
            ViewData["size"] = size;
            ViewData["orderBy"] = orderBy;
            ViewData["direction"] = direction;
            ViewData["totalPages"] = _videoRepository.VideoCount() / size;
            return PartialView("VideoTableBodyPartial",vmVideo);
        }
        public IActionResult FilterVideos(string videoName) 
        {
            var blVideos = _videoRepository.GetFilterDataByName(videoName);
            var vmVideo = _mapper.Map<IEnumerable<VMVideo>>(blVideos);
            foreach (var video in vmVideo)
            {
                var blGenre = _genreRepository.GetGenreByID(video.GenreId);
                video.GenreName = blGenre?.Name;

                var blImage = _imageRepository.GetImageByID(video.ImageId);
                video.ImageContent = blImage?.Content;
            }
            ViewData["page"] = 0;
            if (ViewData.ContainsKey("size"))
            {
                ViewData["size"] = (int)ViewData["size"];
            }
            ViewData["orderBy"] = (string)ViewData["orderBy"];
            ViewData["direction"] = (string)ViewData["direction"];
            if (ViewData.ContainsKey("size"))
            {
                int size = (int)ViewData["size"];
                ViewData["totalPages"] = _videoRepository.VideoCount() / size;
            }
            return PartialView("VideoTableBodyPartial", vmVideo);
        }
        public IActionResult Details(int id)
        {
            var blVideo = _videoRepository.GetVideoById(id);
            if (blVideo == null) 
            {
                return NotFound();
            }
            var vmVideo = _mapper.Map<VMVideo>(blVideo);
            var genreforvideo = _genreRepository.GetGenreByID(vmVideo.GenreId);
            var imageforVideo = _imageRepository.GetImageByID(vmVideo.ImageId);
            ViewBag.GenreName = genreforvideo?.Name;
            ViewBag.ImageName = imageforVideo?.Content;
            var videosTagsforvideo = _videoRepository.GetVideoTagsByVideoId(id);
            var vmVideosTagsForVideo = videosTagsforvideo.ToList();
            ViewBag.VideosTags = vmVideosTagsForVideo; 
            return View(vmVideo);
        }
        
    }
}
