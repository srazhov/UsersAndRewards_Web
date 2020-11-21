using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersAndRewards.Database.Tables;
using UsersAndRewards.Models;

namespace UsersAndRewards.Database
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>().ForMember("PhotoUrl", opt => opt.MapFrom(c => c.Photo));
            CreateMap<UserViewModel, User>().ForMember("Photo", opt => opt.MapFrom(c => c.PhotoUrl));

            CreateMap<Reward, RewardViewModel>().ForMember("ImageUrl", opt => opt.MapFrom(c => c.Image));
            CreateMap<RewardViewModel, Reward>().ForMember("Image", opt => opt.MapFrom(c => c.ImageUrl));
        }
    }
}
