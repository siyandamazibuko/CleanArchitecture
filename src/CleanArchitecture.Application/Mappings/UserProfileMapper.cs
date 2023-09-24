using AutoMapper;
using Entities = CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Mappings
{
    public class UserProfileMapper : Profile
    {
        public UserProfileMapper()
        {
            CreateMap<Models.Users.User, Entities.User>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
