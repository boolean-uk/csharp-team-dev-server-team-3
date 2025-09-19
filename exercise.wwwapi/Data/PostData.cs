using exercise.wwwapi.Models;

namespace exercise.wwwapi.Data
{
    public class PostData
    {
        private List<string> _subject = new List<string>()
        {
            "Just finished",
            "Trying out",
            "Today I deployed",
            "Learning",
            "Exploring",
            "Refactoring",
            "Documented",
            "Testing",
            "Experimenting with",
            "🔥"
        };

        private List<string> _objects = new List<string>
        {
            "my new development environment 🚀",
            "Entity Framework Core",
            "my first API to Azure 🎉",
            "C# async patterns",
            "LINQ queries",
            "a machine learning project",
            "a new UI for my app",
            "a stock trading algorithm designed to cause HAVOC on the stock market",
            "a new toy for my dog",
            "a new feature request",
            "🔥",
            "a mystery novel",
            "Master Chef UK",
            "morning coffee",
            "morning yoga",
            "afternoon tea",
            "chocolate chip cookies",
        };

        private List<string> _endings = new List<string>
        {
            "and it works perfectly!",
            "which was surprisingly tricky.",
            "can't wait to share it!",
            "hoping to improve it soon.",
            "finally solved a major bug 🐛",
            "with some cool new tricks!",
            "and learned a lot along the way.",
            ", nothing works...",
            "and im about to destroy my computer.",
            ", my brain has ceased to exist.",
            "🔥",
            "and it was crazy!"
        };
        DateTime somedate = new DateTime(2020, 12, 05, 0, 0, 0, DateTimeKind.Utc);
        private List<Post> _posts = new List<Post>();
        public PostData(List<User> users)
        {
            Random random = new Random(1);
            
            for (int i = 1; i < users.Count / 5; i++)
            {
                string subject = _subject[random.Next(9 - 1)];
                string obj = _objects[random.Next(16 - 1)];
                string ending = _endings[random.Next(12 - 1)];
                string content = subject + " " + obj + " " + ending;
                int likes = random.Next(0, 100);
                int userid = random.Next(0, 100);

                _posts.Add(new Post
                {
                    Id = i,
                    UserId = users[random.Next(users.Count)].Id,
                    Content = content,
                    CreatedAt = somedate.AddDays(-i),
                    UpdatedAt = null,
                    NumLikes = likes
                });
            }
        }

        public List<Post> Posts { get { return _posts; } }
    }
}
