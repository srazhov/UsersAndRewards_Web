using AutoMapper;
using UsersAndRewards.Database.Tables;
using UsersAndRewards.Models;

namespace UsersAndRewards.Database
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember("PhotoUrl", opt => opt.MapFrom(c => c.Photo))
                .ForMember("RewardsVM", opt => opt.MapFrom(c => c.Rewards));

            CreateMap<UserViewModel, User>().ForMember("Photo", opt => opt.MapFrom(c => c.PhotoUrl));

            CreateMap<Reward, RewardViewModel>().ForMember("ImageUrl", opt => opt.MapFrom(c => c.Image));
            CreateMap<RewardViewModel, Reward>().ForMember("Image", opt => opt.MapFrom(c => c.ImageUrl));
        }
    }
}
