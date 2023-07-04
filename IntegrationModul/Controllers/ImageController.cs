using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using IntegrationModul.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationModul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public ImageController(IImageRepository imageRepository, IMapper mappper)
        {
            _imageRepository = imageRepository;
            _mapper = mappper;
        }
        [HttpPost("[action]")]
        public ActionResult<Image> CreateImage([FromQuery] string content) 
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    return BadRequest("Content is required");
                }
                var image = new Models.Image
                {
                    Content = content
                };
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var blImage = _mapper.Map<BLImage>(image);
                var newImage = _imageRepository.AddImage(blImage);
                var createdImage = _mapper.Map<Image>(newImage);
                return Ok(createdImage);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with creating the image");
            }
        }
        [HttpPut("[action]/{id}")]
        public ActionResult<Image> UpdateTag(int id, [FromQuery] string content)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var blImage = _imageRepository.GetImageByID(id);
                if (blImage == null)
                {
                    return NotFound($"Video for update with ID: {id} is not found");
                }
                blImage.Content = content;
                var updatedImage = _imageRepository.UpdateImage(id, blImage);
                var imagee = _mapper.Map<Image>(updatedImage);
                return Ok(imagee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with updating the image");
            }
        }
        [HttpDelete("[action]/{id}")]
        public ActionResult<Image> DeleteImage(int id)
        {
            try
            {
                var blImage =_imageRepository.GetImageByID(id);
                if (blImage == null)
                {
                    return NotFound($"Image with ID: {id} is not found");
                }
                _imageRepository.DeleteImage(id);
                return Ok(blImage);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with deleting the video, image with that id is connect with other table");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Image>> GetAllImages()
        {
            try
            {
                var blImages = _imageRepository.GetAllImages();
                if (blImages == null)
                {
                    return NotFound($"There are no images in the database");
                }
                var images = _mapper.Map<IEnumerable<Image>>(blImages);
                return Ok(images);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching all images");
            }
        }
        [HttpGet("[action]/{id}")]
        public ActionResult<Image> GetImageByID(int id)
        {
            try
            {
                var blImages = _imageRepository.GetImageByID(id);
                if (blImages == null)
                {
                    return NotFound($"Image with ID: {id} is not found");
                }
                var image = _mapper.Map<Image>(blImages);
                return Ok(image);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching image");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Image>> SearchImagesBySpecificPartOfContent(string part)
        {
            try
            {
                var blImages = _imageRepository.GetAllImages().Where(x => x.Content.ToLower().Contains(part.ToLower()));
                if (blImages == null)
                {
                    return NotFound($"Images with that part is not found");
                }
                var images = _mapper.Map<IEnumerable<Image>>(blImages);
                return Ok(images);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching images");
            }
        }
    }
}
