using AutoMapper;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.DTOs.Posts;
using exercise.wwwapi.Models;

namespace workshop.wwwapi.Tools
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<User, UserBasicDTO>();
            CreateMap<Post, PostDTO>();

            CreateMap<PostComment, PostCommentDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        }
    }
}