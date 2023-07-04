using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repositories
{
    public interface IImageRepository
    {
        IEnumerable<BLImage> GetAllImages();
        BLImage GetImageByID(int id);
        BLImage AddImage(BLImage image);
        BLImage UpdateImage(int id,BLImage image);
        void DeleteImage(int id);
    }

    public class ImageRepository : IImageRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public ImageRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public BLImage AddImage(BLImage image)
        {
            var newDbImage = _mapper.Map<Image>(image);
            newDbImage.Id = 0;
            _dbContext.Images.Add(newDbImage);
            _dbContext.SaveChanges();
            var newBlImage = _mapper.Map<BLImage>(newDbImage);
            return newBlImage;
        }

        public BLImage UpdateImage(int id, BLImage image)
        {
            var dbImage = _dbContext.Images.FirstOrDefault(x => x.Id == id);
            if (dbImage == null)
            {
                throw new InvalidOperationException("Image not found");
            }
            _mapper.Map(image, dbImage);
            _dbContext.SaveChanges();
            var updatedBlImage = _mapper.Map<BLImage>(dbImage);
            return updatedBlImage;
        }

        public void DeleteImage(int id)
        {
            var dbImage = _dbContext.Images.FirstOrDefault(x => x.Id == id);
            if (dbImage == null)
            {
                throw new InvalidOperationException("Image not found");
            }
            _dbContext.Images.Remove(dbImage);
            _dbContext.SaveChanges();
        }

        public IEnumerable<BLImage> GetAllImages()
        {
            var dbImages = _dbContext.Images;
            var blImages = _mapper.Map<IEnumerable<BLImage>>(dbImages);
            return blImages;
        }

        public BLImage GetImageByID(int id)
        {
            var dbImage = _dbContext.Images.FirstOrDefault(x => x.Id == id);
            var blImage = _mapper.Map<BLImage>(dbImage);
            return blImage;
        }
    }
}
