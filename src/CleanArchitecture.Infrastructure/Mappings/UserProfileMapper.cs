using AutoMapper;
using Entities = CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Infrastructure.Mappings
{
    public class UserProfileMapper : Profile
    {
        public UserProfileMapper()
        {
            CreateMap<Models.Users.User, Entities.User>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Models.Users.UserInformation, Entities.User>()
                .ForMember(dst => dst.DateOfBirth, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
