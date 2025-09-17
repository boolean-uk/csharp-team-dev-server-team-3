using AutoMapper;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.Cohort;
using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.DTOs.Posts;
using exercise.wwwapi.DTOs.Register;
using exercise.wwwapi.Models;
using System;
using System.Numerics;

namespace workshop.wwwapi.Tools
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<User, UserBasicDTO>();
            CreateMap<Post, PostDTO>();

            CreateMap<Cohort, CohortDTO>()
                .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.UserCohorts.Where(u => u.User.Role == Roles.student)))
                .ForMember(dest => dest.Teachers, opt => opt.MapFrom(src => src.UserCohorts.Where(u => u.User.Role == Roles.teacher)));

            CreateMap<UserCohort, UserCohortDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            //CreateMap<UserCohortDTO, UserBasicDTO>();

            CreateMap<PostComment, PostCommentDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        }
    }
}