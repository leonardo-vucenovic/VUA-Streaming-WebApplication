using AutoMapper;
using BusinessLayer.BLModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Mapping
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile() 
        {
            CreateMap<DALModels.Country, BLModels.BLCountry>();
            CreateMap<DALModels.Genre, BLModels.BLGenre>();
            CreateMap<DALModels.Image, BLModels.BLImage>();
            CreateMap<DALModels.Notification, BLModels.BLNotification>();
            CreateMap<DALModels.Tag, BLModels.BLTag>();
            CreateMap<DALModels.User, BLModels.BLUser>();
            CreateMap<DALModels.Video, BLModels.BLVideo>();
            CreateMap<DALModels.VideoTag, BLModels.BLVideoTag>();

            CreateMap<BLModels.BLCountry, DALModels.Country>();
            CreateMap<BLModels.BLGenre, DALModels.Genre>();
            CreateMap<BLModels.BLImage, DALModels.Image>();
            CreateMap<BLModels.BLNotification, DALModels.Notification>();
            CreateMap<BLModels.BLTag, DALModels.Tag>();
            CreateMap<BLModels.BLUser, DALModels.User>();
            CreateMap<BLModels.BLVideo, DALModels.Video>();
            CreateMap<BLModels.BLVideoTag, DALModels.VideoTag>();
        }
    }
}
