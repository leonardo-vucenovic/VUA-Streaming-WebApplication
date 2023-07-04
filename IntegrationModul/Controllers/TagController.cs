using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using IntegrationModul.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace IntegrationModul.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagController(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public ActionResult<Tag> CreateTag([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) 
                {
                    return BadRequest("Name is required");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var tag = new Models.Tag
                { 
                    Name = name 
                };
                var blTag = _mapper.Map<BLTag>(tag);
                var newTag = _tagRepository.AddTag(blTag);
                var createdTag = _mapper.Map<Tag>(newTag);
                return Ok(createdTag);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with creating the tag");
            }
        }
        [HttpPut("[action]/{id}")]
        public ActionResult<Tag> UpdateTag(int id, [FromQuery] string name)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var blTag = _tagRepository.GetTagByID(id);
                if (blTag == null)
                {
                    return NotFound($"Tag for update with ID: {id} is not found");
                }
                blTag.Name = name;
                var updatedTag = _tagRepository.UpdateTag(id, blTag);
                var tagg = _mapper.Map<Tag>(updatedTag);
                return Ok(tagg);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with updating the tag");
            }
        }
        [HttpDelete("[action]/{id}")]
        public ActionResult DeleteTag(int id)
        {
            try
            {
                var blTag = _tagRepository.GetTagByID(id);
                if (blTag == null)
                {
                    return NotFound($"Tag with ID: {id} is not found");
                }
                var deletedTag = _tagRepository.DeleteTag(blTag);
                return Ok(deletedTag);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with deleting the tag, tag with that id is connect with other table");
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Tag>> GetAllTags() 
        {
            try
            {
                var blTags = _tagRepository.GetAllTags();
                if (blTags == null)
                {
                    return NotFound($"There are no tags in the database");
                }
                var tags = _mapper.Map<IEnumerable<Tag>>(blTags);
                return Ok(tags);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching all tags");
            }
        }
        [HttpGet("[action]/{id}")]
        public ActionResult<Tag> GetTagByID(int id) 
        {
            try
            {
                var blGenre = _tagRepository.GetTagByID(id);
                if (blGenre == null)
                {
                    return NotFound($"Tag with ID: {id} is not found");
                }
                var tag = _mapper.Map<Tag>(blGenre);
                return Ok(tag);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching tag");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Tag>> SearchTagsBySpecificPartOfName(string part) 
        {
            try
            {
                var blTags = _tagRepository.GetAllTags().Where(x => x.Name.ToLower().Contains(part.ToLower()));
                if (blTags == null)
                {
                    return NotFound($"Tags with that part is not found");
                }
                var tags = _mapper.Map<IEnumerable<Tag>>(blTags); 
                return Ok(tags);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching tag");
            }
        }
    }
}
