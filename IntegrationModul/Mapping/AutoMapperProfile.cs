using AutoMapper;

namespace IntegrationModul.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BusinessLayer.BLModels.BLCountry, Models.Country>();
            CreateMap<BusinessLayer.BLModels.BLGenre, Models.Genre>();
            CreateMap<BusinessLayer.BLModels.BLImage, Models.Image>();
            CreateMap<BusinessLayer.BLModels.BLNotification, Models.Notification>();
            CreateMap<BusinessLayer.BLModels.BLTag, Models.Tag>();
            CreateMap<BusinessLayer.BLModels.BLUser, Models.User>();
            CreateMap<BusinessLayer.BLModels.BLVideo, Models.Video>();
            CreateMap<BusinessLayer.BLModels.BLVideoTag, Models.VideoTag>();

            CreateMap<IntegrationModul.Models.Country, BusinessLayer.BLModels.BLCountry>();
            CreateMap<IntegrationModul.Models.Genre, BusinessLayer.BLModels.BLGenre>();
            CreateMap<IntegrationModul.Models.Image, BusinessLayer.BLModels.BLImage>();
            CreateMap<IntegrationModul.Models.Notification, BusinessLayer.BLModels.BLNotification>();
            CreateMap<IntegrationModul.Models.Tag, BusinessLayer.BLModels.BLTag>();
            CreateMap<IntegrationModul.Models.User, BusinessLayer.BLModels.BLUser>();
            CreateMap<IntegrationModul.Models.Video, BusinessLayer.BLModels.BLVideo>();
            CreateMap<IntegrationModul.Models.VideoTag, BusinessLayer.BLModels.BLVideoTag>();
        }
    }
}
