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
            CreateMap<Cohort, CohortDTO>();

            CreateMap<UserCohort, UserCohortDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Cohort, opt => opt.MapFrom(src => src.Cohort.Title));
            //CreateMap<UserCohortDTO, UserBasicDTO>();

            CreateMap<PostComment, PostCommentDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        }
    }
}