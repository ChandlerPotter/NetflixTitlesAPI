using System;
using AutoMapper;

namespace NetflixTitles.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Entities.User, Models.UserDto>();
            CreateMap<Entities.User, Models.UserWithoutListsDto>();
            CreateMap<Models.RegisterUserDto, Entities.User>();
        }
    }
}

