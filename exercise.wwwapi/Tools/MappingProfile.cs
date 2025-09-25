using AutoMapper;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.DTOs.Cohort;
using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.DTOs.Posts;
using exercise.wwwapi.Models;

namespace workshop.wwwapi.Tools
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Might change this if we want to return all the cohorts a user is in OR the active cohort
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Cohort, opt => opt.MapFrom(src => src.CohortCourseUsers.FirstOrDefault().Cohort)); 

            CreateMap<User, UserBasicDTO>();

            CreateMap<Post, PostDTO>();

            CreateMap<Cohort, BasicCohortDTO>();

            CreateMap<Cohort, CohortDTO>()
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.CohortCourses));

            CreateMap<CohortCourseUser, CohortCourseUserDTO>()
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.Cohort, opt => opt.MapFrom(src => src.Cohort.Title));

            CreateMap<CohortCourse, CourseInCohortDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.Students, opt => opt.MapFrom(
                    src => src.CohortCourseUsers
                        .Where(ccu => ccu.User.Role == Roles.student)
                        .Select(ccu => ccu.User)))
                .ForMember(dest => dest.Teachers, opt => opt.MapFrom(
                    src => src.CohortCourseUsers
                        .Where(ccu => ccu.User.Role == Roles.teacher)
                        .Select(ccu => ccu.User)));

            CreateMap<PostComment, PostCommentDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        }
    }
}