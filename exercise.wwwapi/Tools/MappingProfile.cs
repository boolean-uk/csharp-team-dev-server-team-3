using AutoMapper;
using exercise.wwwapi.DTOs;
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

        }
    }
}