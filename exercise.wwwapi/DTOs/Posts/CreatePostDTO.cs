using exercise.wwwapi.DTOs.GetUsers;
using exercise.wwwapi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exercise.wwwapi.DTOs.Posts
{
    public class CreatePostDTO
    {
        public required int Userid { get; set; }
        public required string Content { get; set; }

    }
}
