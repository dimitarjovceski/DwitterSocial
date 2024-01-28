using AutoMapper;
using DwitterSocial.Dtos;
using DwitterSocial.Entities;
using DwitterSocial.Extensions;

namespace DwitterSocial.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(p => p.PhotoUrl, o => o.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember( p=> p.Age, o => o.MapFrom( s=> s.DateOfBirth.CalculateAge()))
                .ReverseMap();
            CreateMap<Photo, PhotoDto>();
            CreateMap<UpdateMemberDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(p => p.SenderPhotoUrl, o => o.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(p => p.ReceiverPhotoUrl, o => o.MapFrom(src => src.Receiver.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        }
    }
}
