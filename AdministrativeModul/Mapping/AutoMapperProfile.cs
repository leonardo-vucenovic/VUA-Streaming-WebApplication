using AutoMapper;

namespace AdministrativeModul.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BusinessLayer.BLModels.BLCountry, ViewModels.VMCountry>();
            CreateMap<BusinessLayer.BLModels.BLGenre, ViewModels.VMGenre>();
            CreateMap<BusinessLayer.BLModels.BLImage, ViewModels.VMImage>();
            CreateMap<BusinessLayer.BLModels.BLNotification, ViewModels.VMNotification>();
            CreateMap<BusinessLayer.BLModels.BLTag, ViewModels.VMTag>();
            CreateMap<BusinessLayer.BLModels.BLUser, ViewModels.VMUser>();
            CreateMap<BusinessLayer.BLModels.BLVideo, ViewModels.VMVideo>();
            CreateMap<BusinessLayer.BLModels.BLVideoTag, ViewModels.VMVideoTag>();

            CreateMap<ViewModels.VMCountry, BusinessLayer.BLModels.BLCountry>();
            CreateMap<ViewModels.VMGenre, BusinessLayer.BLModels.BLGenre>();
            CreateMap<ViewModels.VMImage, BusinessLayer.BLModels.BLImage>();
            CreateMap<ViewModels.VMNotification, BusinessLayer.BLModels.BLNotification>();
            CreateMap<ViewModels.VMTag, BusinessLayer.BLModels.BLTag>();
            CreateMap<ViewModels.VMUser, BusinessLayer.BLModels.BLUser>();
            CreateMap<ViewModels.VMVideo, BusinessLayer.BLModels.BLVideo>();
            CreateMap<ViewModels.VMVideoTag, BusinessLayer.BLModels.BLVideoTag>();
            CreateMap<BusinessLayer.BLModels.BLVideo, ViewModels.VMVideoEdit>();
        }
    }
}
