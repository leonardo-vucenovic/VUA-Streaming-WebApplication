using AdministrativeModul.ViewModels;
using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace AdministrativeModul.Controllers
{
    public class TagController : Controller
    {
        private readonly ILogger<TagController> _logger;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagController(ILogger<TagController> logger, ITagRepository tagRepo, IMapper mapper)
        {
            _logger = logger;
            _tagRepository = tagRepo;
            _mapper = mapper;
        }
        public IActionResult Tag()
        {
            var blTags = _tagRepository.GetAllTags();
            var vmTags = _mapper.Map<IEnumerable<VMTag>>(blTags);
            return View(vmTags);
        }
        public IActionResult Details(int id)
        {
            var blTag = _tagRepository.GetTagByID(id);
            if (blTag == null)
            {
                return NotFound();
            }
            var vmTag = _mapper.Map<VMTag>(blTag);
            return View(vmTag);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(VMTag tag)
        {
            try
            {
                var blTag = _mapper.Map<BLTag>(tag);
                var newt = _tagRepository.AddTag(blTag);
                var vmtag = _mapper.Map<VMTag>(newt);
                return RedirectToAction(nameof(Tag));
            }
            catch
            {
                return View(tag);
            }
        }
        public IActionResult Edit(int id)
        {
            var blTag = _tagRepository.GetTagByID(id);
            if (blTag == null) 
            {
                return NotFound(); 
            }
            var vmTag = _mapper.Map<VMTag>(blTag);
            return View(vmTag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, VMTag tag)
        {
            try
            {
                var blTag = _mapper.Map<BLTag>(tag);
                _tagRepository.UpdateTag(id, blTag);
                return RedirectToAction(nameof(Tag));
            }
            catch
            {
                return View(tag);
            }
        }

        public ActionResult Delete(int id)
        {
            var blTag = _tagRepository.GetTagByID(id);
            if (blTag == null)
            {
                return NotFound();
            }
            var deletedtag = new VMTag
            {
                Id = blTag.Id,
                Name = blTag.Name,
            };
            return View(deletedtag);
        }
        [HttpPost]
        public ActionResult Delete(int id, VMTag deletetag)
        {
            try
            {
                var tag = _tagRepository.GetTagByID(id);
                if (tag == null)
                {
                    return NotFound();
                }
                var bltag = _tagRepository.DeleteTag(tag);
                return RedirectToAction(nameof(Tag));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Tag));
            }
        }
    }
}