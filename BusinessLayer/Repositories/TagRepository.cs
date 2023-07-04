using AutoMapper;
using Azure;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<BLTag> GetAllTags();
        BLTag GetTagByID(int id);
        BLTag AddTag(BLTag tag);
        BLTag UpdateTag(int id,BLTag tag);
        BLTag DeleteTag(BLTag bLTag);
    }

    public class TagRepository : ITagRepository
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public TagRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public BLTag AddTag(BLTag tag)
        {
            var dbTag = _mapper.Map<Tag>(tag);
            dbTag.Id = 0;
            _dbContext.Tags.Add(dbTag);
            _dbContext.SaveChanges();
            var blTag = _mapper.Map<BLTag>(dbTag);
            return blTag;
        }

        public BLTag UpdateTag(int id, BLTag tag)
        {
            var dbTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);
            if (dbTag == null) 
            {
                throw new InvalidOperationException("Tag not found");
            }
            _mapper.Map(tag,dbTag);
            _dbContext.SaveChanges();
            var updatedBlTag = _mapper.Map<BLTag>(dbTag);
            return updatedBlTag;
        }

        public BLTag DeleteTag(BLTag tag)
        {
            using (var databaseTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var cbTag = _dbContext.Tags.FirstOrDefault(x => x.Id == tag.Id);
                    if (cbTag == null)
                    {
                        throw new InvalidOperationException("Tag not found");
                    }
                    var dbvideostahgs = _dbContext.VideoTags.Where(x => x.TagId == tag.Id);
                    _dbContext.VideoTags.RemoveRange(dbvideostahgs);
                    _dbContext.SaveChanges();
                    _dbContext.Tags.RemoveRange(cbTag);
                    _dbContext.SaveChanges();
                    databaseTransaction.Commit();
                    return _mapper.Map<BLTag>(cbTag);
                }
                catch (Exception)
                {
                    databaseTransaction.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<BLTag> GetAllTags()
        {
            var dbTags = _dbContext.Tags;
            var blTags = _mapper.Map<IEnumerable<BLTag>>(dbTags);
            return blTags;
        }

        public BLTag GetTagByID(int id)
        {
            var dbTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);
            var blTag = _mapper.Map<BLTag>(dbTag);
            return blTag;
        }
    }
}