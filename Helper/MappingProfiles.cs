using AutoMapper;
using SecretSantaApi.Dto;
using SecretSantaApi.Models;

namespace SecretSantaApi.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<SetPasswordDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore());
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserLoginDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore()); // Ignore Password field
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // Ignore Password field
            CreateMap<User, UserDto>();
            CreateMap<GiftList, GiftListDto>();
            CreateMap<GiftListDto, GiftList>();
            CreateMap<PersonUpdateDto, Person>()
                 .ForMember(dest => dest.ListId, opt => opt.Ignore())
                 .ForMember(dest => dest.IsBuyer, opt => opt.Ignore())
                 .ForMember(dest => dest.giverGiftee, opt => opt.Ignore());// Ignore ListId field
            CreateMap<Person, PersonDto>();
            CreateMap<PersonDto, Person>();
        }
    }
}
