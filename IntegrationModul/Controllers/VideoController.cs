using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using IntegrationModul.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using System.Threading.Tasks.Dataflow;

namespace IntegrationModul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IMapper _mapper;
        private readonly ITagRepository _tagRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IImageRepository _imageRepository;

        public VideoController(IVideoRepository videoRepositor, IMapper mapper, ITagRepository tagRepository, IGenreRepository genreRepository, IImageRepository imageRepository)
        {
            _videoRepository = videoRepositor;
            _mapper = mapper;
            _tagRepository = tagRepository;
            _imageRepository = imageRepository;
            _genreRepository = genreRepository;

        }
        [HttpPost("[action]")]
        public ActionResult<Video> CreateVideo([FromQuery] string name, [FromQuery] string description, [FromQuery] int genreID, [FromQuery] int totalSeconds, [FromQuery] string streamURL, [FromQuery] int imageID, [FromQuery] List<int> tagsID)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("Name is required.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var video = new Video
                {
                    Name = name,
                    Description = description,
                    GenreId = genreID,
                    TotalSeconds = totalSeconds,
                    StreamingUrl = streamURL,
                    ImageId = imageID,
                    Genre = null,
                    Image = null,
                    VideoTags = new List<VideoTag>()
                };
                if (tagsID != null && tagsID.Any())
                {
                    foreach (var id in tagsID)
                    {
                        var videoTag = new VideoTag
                        {
                            VideoId = video.Id,
                            TagId = id
                        };
                        video.VideoTags.Add(videoTag);
                    }
                }
                var blVideo = _mapper.Map<BLVideo>(video);
                var newVideo = _videoRepository.AddVideo(blVideo);
                var createdVideo = _mapper.Map<Video>(newVideo);
                return Ok(newVideo);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with creating the video, some of IDs does not exists.");
            }
        }
        [HttpPut("[action]")]
        public ActionResult<Video> UpdateVideo(int id, [FromQuery] string name, [FromQuery] string description, [FromQuery] int genreID, [FromQuery] int totalSeconds, [FromQuery] string streamURL, [FromQuery] int imageID, [FromQuery] List<int> tagsID)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var blVideo = _videoRepository.GetVideoById(id);
                if (blVideo == null)
                {
                    return NotFound($"Video for update with ID: {id} is not found.");
                }
                blVideo.Name = name;
                blVideo.Description = description;
                blVideo.GenreId = genreID;
                blVideo.TotalSeconds = totalSeconds;
                blVideo.StreamingUrl = streamURL;
                blVideo.ImageId = imageID;
                if (tagsID != null)
                {
                    foreach (var iddd in tagsID)
                    {
                        var tag = _videoRepository.GetVideoById(id);
                        var videostags = blVideo.VideoTags.FirstOrDefault(x => x.VideoId == id);
                        blVideo.VideoTags.Remove(videostags);
                        if (_tagRepository.GetTagByID(iddd) == null)
                        {
                            return NotFound($"VideoTag with ID: {iddd} is not found.");
                        }
                        else
                        {
                            var videoTag = new VideoTag
                            {
                                VideoId = blVideo.Id,
                                TagId = iddd
                            };
                            var videoTagbl = _mapper.Map<BLVideoTag>(videoTag);
                            blVideo.VideoTags.Add(videoTagbl);
                        }

                    }
                }
                var updatedVideo = _videoRepository.UpdateVideo(id, blVideo);
                var videoo = _mapper.Map<Video>(updatedVideo);
                return Ok(videoo);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with updating the video, some of IDs does not exists.");
            }
        }
        [HttpDelete("[action]/{id}")]
        public ActionResult<Video> DeleteVideo(int id)
        {
            try
            {
                var blVideo = _videoRepository.GetVideoById(id);
                if (blVideo == null)
                {
                    return NotFound($"Video for update with ID: {id} is not found.");
                }
                _videoRepository.DeleteVideo(id);
                return Ok(blVideo);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with deletion the video.");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Video>> GetAllVideos()
        {
            try
            {
                var blVideos = _videoRepository.GetAllVideos();
                if (blVideos == null)
                {
                    return NotFound($"There are no videos in the database.");
                }
                var videos = _mapper.Map<IEnumerable<Video>>(blVideos);
                return Ok(videos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching all videos.");
            }
        }
        [HttpGet("[action]/{id}")]
        public ActionResult<Video> GetVideoByID(int id)
        {
            try
            {
                var blVideo = _videoRepository.GetVideoById(id);
                if (blVideo == null)
                {
                    return NotFound($"Video with ID: {id} is not found.");
                }
                var videoo = _mapper.Map<Video>(blVideo);
                return Ok(videoo);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching video.");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Video>> SearchVideosBySpecificPartOfName(string part)
        {
            try
            {
                var blVideo = _videoRepository.GetAllVideos().Where(x => x.Name.ToLower().Contains(part.ToLower()));
                if (blVideo == null)
                {
                    return NotFound($"Video with that part is not found");
                }
                var videoss = _mapper.Map<IEnumerable<Video>>(blVideo);
                return Ok(videoss);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching video.");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Video>> GetAllVideoWithFilerPageAndDirection(string Filer_Name_Or_Part_Of_Name,string Choose_Sort_Name_Or_TotalTime = "id",string Direction_asc_or_desc = "asc", int page = 1, int pageSize = 10)
        {
            try
            {
                var blVideo = _videoRepository.GetAllVideos();
                var videos = _mapper.Map<IEnumerable<Video>>(blVideo);
                if (Filer_Name_Or_Part_Of_Name == null || Choose_Sort_Name_Or_TotalTime == null || Direction_asc_or_desc == null || page == null || pageSize == null)
                {
                    return NotFound($"Choose name or part of name and do not delete default value for rest of data.");
                }
                videos = videos.Where(v => v.Name.ToLower().Contains(Filer_Name_Or_Part_Of_Name.ToLower()));
                switch (Choose_Sort_Name_Or_TotalTime)
                {
                    case "name":
                        videos = videos.OrderBy(v => v.Name);
                        break;
                    case "totaltime":
                        videos = videos.OrderBy(v => v.TotalSeconds);
                        break;
                    default:
                        videos = videos.OrderBy(v => v.Id);
                        break;
                }
                switch (Direction_asc_or_desc)
                {
                    case "desc":
                        videos = videos.Reverse();
                        break;
                    default:
                        videos = videos;
                        break;
                }
                videos = videos.Skip(page * pageSize).Take(pageSize);
                return Ok(videos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching all videos with yours rules.");
            }
        }
    }
}
