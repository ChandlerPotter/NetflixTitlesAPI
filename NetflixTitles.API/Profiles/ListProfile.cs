using System;
using AutoMapper;

namespace NetflixTitles.API.Profiles
{
    public class ListProfile :Profile
    {
        public ListProfile()
        {
            CreateMap<Entities.List, Models.ListDto>()
               .ForMember(dest => dest.Titles,
               opts => opts.MapFrom(src => src.TitleLists))
               .ForMember(dest => dest.UserName,
               opts => opts.MapFrom(src => src.User.UserName));

            CreateMap<Entities.List, Models.ListWithoutTitlesDto>()
               .ForMember(dest => dest.UserName,
               opts => opts.MapFrom(src => src.User.UserName)); ;

            CreateMap<Models.ListForCreationDto, Entities.List>();

            CreateMap<Entities.List, Models.ListForUpdateDto>();

            CreateMap<Models.ListForUpdateDto, Entities.List>();
        }
    }
}

