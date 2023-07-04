using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BusinessLayer.Repositories
{
    public interface IVideoRepository
    {
        IEnumerable<BLVideo> GetAllVideos();
        BLVideo GetVideoById(int id);
        BLVideo AddVideo(BLVideo video);
        BLVideo UpdateVideo(int id,BLVideo video);
        void DeleteVideo(int id);
        int VideoCount();
        Dictionary<string, int> GetGenreMappings();
        Dictionary<int, string> GetGenres();
        IEnumerable<BLVideo> GetPagedData(int page, int size, string orderBy, string direction);
        IEnumerable<BLVideo> GetFilteredData(string name, string genre);
        public IEnumerable<string> GetVideoTagsByVideoId(int videoId);
        public BLVideo AddVideoBL(string name, string description, int genreid, int imageid, int totalSeconds, string streamingUrl);
        IEnumerable<BLVideo> GetFilterDataByName(string videoName);
    }

    public class VideoRepository : IVideoRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public VideoRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public BLVideo AddVideo(BLVideo video)
        {
            var newDbVideo = _mapper.Map<Video>(video);
            newDbVideo.Id = 0;
            _dbContext.Videos.Add(newDbVideo);
            _dbContext.SaveChanges();
            var newBlVideo = _mapper.Map<BLVideo>(newDbVideo);
            return newBlVideo;
        }

        public BLVideo UpdateVideo(int id, BLVideo video)
        {
            var dbVideo = _dbContext.Videos.FirstOrDefault(x => x.Id == id);
            if (dbVideo == null)
            {
                throw new InvalidOperationException("Video not found");
            }
            _mapper.Map(video, dbVideo);
            _dbContext.SaveChanges();
            var updatedBlVideo = _mapper.Map<BLVideo>(dbVideo);
            
            return updatedBlVideo;
        }

        public void DeleteVideo(int id)
        {
            using (var databaseTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dbVideo = _dbContext.Videos.FirstOrDefault(x => x.Id == id);
                    if (dbVideo == null) 
                    {
                        throw new InvalidOperationException("Video not found");
                    }
                    var videoTags = _dbContext.VideoTags.Where(x => x.VideoId == id);
                    _dbContext.VideoTags.RemoveRange(videoTags);
                    _dbContext.SaveChanges();
                    _dbContext.Videos.Remove(dbVideo);
                    _dbContext.SaveChanges();
                    databaseTransaction.Commit();
                }
                catch (Exception)
                {
                    databaseTransaction.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<BLVideo> GetAllVideos()
        {
            var dbVideos = _dbContext.Videos;
            var blVideos = _mapper.Map<IEnumerable<BLVideo>>(dbVideos);
            return blVideos;
        }

        public BLVideo GetVideoById(int id)
        {
            var dbVideo = _dbContext.Videos.FirstOrDefault(x => x.Id == id);
            var blVideo = _mapper.Map<BLVideo>(dbVideo);
            return blVideo;
        }

        public int VideoCount()
        {
            return _dbContext.Videos.Count();
        }
        public Dictionary<string, int> GetGenreMappings()
        {
            var genreMappings = new Dictionary<string, int>();
            var genres = _dbContext.Genres.ToList();
            foreach (var genre in genres)
            {
                genreMappings.Add(genre.Name,genre.Id);
            }
            return genreMappings;
        }
        public Dictionary<int, string> GetGenres()
        {
            var genreMappings = new Dictionary<int, string>();
            var genres = _dbContext.Genres.ToList();
            foreach (var genre in genres)
            {
                genreMappings.Add(genre.Id,genre.Name);
            }
            return genreMappings;
        }

        public IEnumerable<BLVideo> GetPagedData(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Video> dbvidoe = _dbContext.Videos.AsEnumerable();
            if (string.Compare(orderBy, "id", true) == 0)
            {
                dbvidoe = dbvidoe.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", true) == 0)
            {
                dbvidoe = dbvidoe.OrderBy(x => x.Name);
            }
            else if (string.Compare(orderBy, "description", true) == 0)
            {
                dbvidoe = dbvidoe.OrderBy(x => x.Description);
            }
            else if (string.Compare(orderBy, "totalSeconds", true) == 0)
            {
                dbvidoe = dbvidoe.OrderBy(x => x.TotalSeconds);
            }
            else if (string.Compare(orderBy, "genreId", true) == 0)
            {
                dbvidoe = dbvidoe.OrderBy(x => x.GenreId);
            }
            else if (string.Compare(orderBy, "imageId", true) == 0)
            {
                dbvidoe = dbvidoe.OrderBy(x => x.ImageId);
            }
            else if (string.Compare(orderBy, "createdAt", true) == 0)
            {
                dbvidoe = dbvidoe.OrderBy(x => x.CreatedAt);
            }
            else
            {
                dbvidoe = dbvidoe.OrderBy(x => x.Id);
            }
            if (string.Compare(orderBy, "desc", true) == 0)
            {
                dbvidoe = dbvidoe.Reverse();
            }
            dbvidoe = dbvidoe.Skip(page * size).Take(size);
            var blVideos = _mapper.Map<IEnumerable<BLVideo>>(dbvidoe);
            return blVideos;
        }

        public IEnumerable<BLVideo> GetFilteredData(string name, string genre)
        {
            var dbvideos = _dbContext.Videos.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                dbvideos = dbvideos.Where(x => x.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(genre))
            {
                var genredIDS = _dbContext.Genres.Where(x => x.Name.Contains(genre)).Select(x => x.Id);
                dbvideos = dbvideos.Where(x => genredIDS.Contains(x.GenreId));
            }
            var blVideo = _mapper.Map<IEnumerable<BLVideo>>(dbvideos);
            return blVideo;
        }

        public IEnumerable<string> GetVideoTagsByVideoId(int videoId)
        {
            var videosTags = _dbContext.VideoTags.Where(x => x.VideoId == videoId).Select(x => x.TagId).ToList();
            var tags = _dbContext.Tags.Where(x => videosTags.Contains(x.Id)).Select(x => x.Name).ToList();
            return tags ?? Enumerable.Empty<string>();
        }

        public BLVideo AddVideoBL(string name, string description, int genreid, int imageid, int totalSeconds, string streamingUrl)
        {
            var dbVideo = new Video()
            {
                CreatedAt = DateTime.Now,
                Name = name,
                Description = description,
                GenreId = genreid,
                ImageId = imageid,
                TotalSeconds = totalSeconds,
                StreamingUrl = streamingUrl
            };
            _dbContext.Videos.Add(dbVideo);
            _dbContext.SaveChanges();
            var blVideo = _mapper.Map<BLVideo>(dbVideo);
            return blVideo;
        }

        public IEnumerable<BLVideo> GetFilterDataByName(string videoName)
        {
            var dbVideo = _dbContext.Videos.Where(x => x.Name.Contains(videoName));
            var blVideo = _mapper.Map<IEnumerable<BLVideo>>(dbVideo);
            return blVideo;
        }
    }
}
