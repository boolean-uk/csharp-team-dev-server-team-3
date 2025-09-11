using exercise.wwwapi.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.InteropServices.JavaScript;

namespace exercise.wwwapi.Data
{
    public class PostCommentData
    {
        private List<string> _commentStarts = new List<string>
        {
            "Wow!",
            "That sounds amazing",
            "I love it",
            "So cool",
            "Congrats",
            "Looks fun",
            "Nice!",
            "That’s awesome",
            "I wish I could",
            "Amazing experience",
            "Haha",
            "Incredible",
            "So relaxing",
            "Beautiful",
            "Great job"
        };
        List<string> _commentMiddles = new List<string>
        {
            "I’ve always wanted to try that",
            "where did you go?",
            "how did it go?",
            "can you share more details?",
            "sounds like a lot of fun",
            "I need to do that too",
            "hope I can join next time",
            "looks so peaceful",
            "that must have been exciting",
            "I want to try this as well",
            "so inspiring",
            "that’s a good idea",
            "sounds delicious",
            "love this suggestion",
            "can’t wait to try it"
        };
        List<string> _commentEnds = new List<string>
        {
            "😄",
            "👍",
            "💖",
            "🌟",
            "🎉",
            "🙌",
            "🍀",
            "😊",
            "👏",
            "🔥",
            "💡",
            "🌸",
            "✨",
            "🎶",
            "🌞",
            "",
            "",
            "!",
            ".",
            ""
        };
        private List<PostComment> _comments = new List<PostComment>();
        public PostCommentData(List<Post> posts, List<User> users)
        {
            int commentId = 1;
            var random = new Random();
            DateTime somedate = new DateTime(2020, 12, 05, 0, 0, 0, DateTimeKind.Utc);
            foreach (var post in posts)
            {
                int numComments = random.Next(0, 5);

                for (int i = 0; i < numComments; i++)
                {
                    var start = _commentStarts[random.Next(_commentStarts.Count)];
                    var middle = _commentMiddles[random.Next(_commentMiddles.Count)];
                    var end = _commentEnds[random.Next(_commentEnds.Count)];

                    int commenterId;
                    do
                    {
                        commenterId = users[random.Next(users.Count())].Id;
                    } while (commenterId == post.UserId);

                    _comments.Add(new PostComment
                    {
                        Id = commentId++,
                        PostId = post.Id,
                        UserId = commenterId,
                        Content = $"{start}, {middle} {end}",
                        CreatedAt = post.CreatedAt.AddMinutes(random.Next(5, 240)),
                        UpdatedAt = null
                    });
                }
            }
        }
        public List<PostComment> Comments { get { return _comments; } }

    }
}
