using System;
using AutoMapper;

namespace NetflixTitles.API.Profiles
{
    public class TitleProfile : Profile
    {
        public TitleProfile()
        {
            CreateMap<Entities.Title, Models.TitleDto>();

            CreateMap<Entities.Title, Models.AllTitlesDto>();

            CreateMap<Entities.TitleList, Models.TitleDto>()
               .ForMember(dest => dest.TitleId,
               opts => opts.MapFrom(src => src.Title!.TitleId))
               .ForMember(dest => dest.TitleName,
               opts => opts.MapFrom(src => src.Title!.TitleName))
               .ForMember(dest => dest.Type,
               opts => opts.MapFrom(src => src.Title!.Type))
               .ForMember(dest => dest.Director,
               opts => opts.MapFrom(src => src.Title!.Director))
               .ForMember(dest => dest.Cast,
               opts => opts.MapFrom(src => src.Title!.Cast))
               .ForMember(dest => dest.Country,
               opts => opts.MapFrom(src => src.Title!.Country))
               .ForMember(dest => dest.DateAdded,
               opts => opts.MapFrom(src => src.Title!.DateAdded))
               .ForMember(dest => dest.ReleaseYear,
               opts => opts.MapFrom(src => src.Title!.ReleaseYear))
               .ForMember(dest => dest.Rating,
               opts => opts.MapFrom(src => src.Title!.Rating))
               .ForMember(dest => dest.Duration,
               opts => opts.MapFrom(src => src.Title!.Duration))
               .ForMember(dest => dest.ListedIn,
               opts => opts.MapFrom(src => src.Title!.ListedIn))
               .ForMember(dest => dest.Description,
               opts => opts.MapFrom(src => src.Title!.Description))
               .ForMember(dest => dest.UserRating,
               opts => opts.MapFrom(src => src.UserRating))
               .ForMember(dest => dest.Watched,
               opts => opts.MapFrom(src => src.Watched));

            CreateMap<Models.TitleForCreationDto, Entities.TitleList>();

            CreateMap<Entities.TitleList, Models.TitleForUpdateDto>();
            
            CreateMap<Models.TitleForUpdateDto, Entities.TitleList>();
        }
    }
}

