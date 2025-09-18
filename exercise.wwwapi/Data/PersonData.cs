using exercise.wwwapi.Models;
using Microsoft.AspNetCore.SignalR;
using static System.Net.WebRequestMethods;

namespace exercise.wwwapi.Data
{
    public class PersonData
    {
        private List<string> _firstnames = new List<string>()
        {
            "Audrey", "Donald", "Elvis", "Barack",
            "Orlando", "Jimi", "Mick", "Kate",
            "Charles", "Kate", "Oyvind Timian", "Rafael",
            "Oyvind", "Timian", "James", "Roger",
            "Roman", "Hans Jakob", "Hans", "Jakob",
            "Vegard", "Jonna", "Jonathan", "Reduan",
            "Chris", "Nigel", "Christian", "Kristoffer",
            "Johnny", "Will",  "Jim", "Taylor",
            "Nicolas", "Nick", "Brad", "Justin",
            "Hillary", "Jennifer", "Anna", "Anne",
            "Marie", "Ingrid", "Inger", "Astrid",
            "Kari", "Solveig", "Ingeborg", "Marit",
            "Ole", "Arne", "Jan", "Per",
            "Johan", "Lars", "Bjorn", "Olav",
            "Knut", "Nora", "Lucas", "Emma",
            "Olivia", "Noa", "Sofie", "Johannes"
        };

        private List<string> _lastnames = new List<string>()
        {
            "Hepburn", "Trump", "Presley", "Obama",
            "Winfrey", "Hendrix", "Jagger", "Winslet",
            "Windsor", "Middleton","Federer", "Johansen",
            "Hansen", "Nadal", "Bond", "Haaland",
            "Olsen", "Larsen", "Andersen", "Pedersen",
            "Nilsen", "Jensen", "Kristiansen", "Risa",
            "Roland", "Stormo", "Beck", "Giske",
            "Holme", "Holm", "Fauske", "Dokken",
            "Gronningen","Soeyland", "Lier", "Vedvik",
            "Gruber", "Meyer","Schmid","Weber",
            "Abdullayev", "Huseynov", "Ceferov", "Peeters",
            "De Smet", "Claes", "Mertens","Delemovic",
            "Horvat", "Novak", "Tamm", "Saar", "Sepp",
            "Ivanov", "Pavlov", "Korhonen", "Laine", "Bernard",
            "Dubois", "Durand", "Moreau", "Robert",
            "Muller", "Schmidt", "Hoffmann", "Thorlacius", "Walsh", "Kelly",
            "OSullivan", "Murphy","Rossi", "Russo",
            "Ferrari", "Esposito","Morina", "Braun",
            "Faber","Borg","Rusu","De Jong", "De Vries",
            "Bakker","Stojanovski", "Nowak","Silva","Popa",
            "Golob", "Garcia", "Gonzalez", "Andersson","Bianchi",
            "Kaya", "Melnyk", "Boyko", "Smith", "Williams", "Brown",
            "Perez","Lopez", "Rodriguez", "Grigoryan",
            "Kim","Lee","dela Cruz","Reyes",
            "Ramos","Perera", "de Silva", "Rathnayake",
        };
        private List<string> _specialisms = new List<string> { "Software Developer", "Data Scientist", "DevOps Engineer", "Frontend Developer" };
        private List<User> _users = new List<User>();
        private List<string> _photoUrls = new List<string>();
        private string baseUrl = "https://mockmind-api.uifaces.co/content/";
        private Dictionary<string, int> urls = new Dictionary<string, int> {
            { "human", 223 },
            {"alien", 18 },
            {"cartoon", 33 },
            {"abstract", 51 }
        };
        private List<string> _passwordHashes = new List<string> {
            "$2a$11$mbfii1SzR9B7ZtKbYydLOuAPBSA2ziAP0CrsdU8QgubGo2afw7Wuy", // Timianerkul1!
            "$2a$11$5ttNr5DmMLFlyVVv7PFkQOhIstdGTBmSdhMHaQcUOZ8zAgsCqFT6e", // SuperHash!4
            "$2a$11$KBLC6riEn/P78lLCwyi0MO9DrlxapLoGhCfdgXwLU2s44P.StKO/6", // Neidintulling!l33t
            "$2a$11$DFMtyLv243uk2liVbzCxXeshisouexhitDg5OUuBU.4LVn//QG5O."  // lettPassord123!
        };

        DateTime somedate = new DateTime(2020, 12, 05, 0, 0, 0, DateTimeKind.Utc);

        public PersonData()
        {
            Random userRandom = new Random(1);

            List<string> allPhotos = new List<string>();
            foreach (var (key, value) in urls)
            {
                for (int i = 1; i <= value; i++)
                {
                    allPhotos.Add($"{baseUrl}{key}/{i}.jpg");
                }
            }

            // shuffle the photos
            allPhotos = allPhotos.OrderBy(_ => userRandom.Next()).ToList();

            for (int x = 1; x < 300; x++)
            {
                string FirstName = _firstnames[userRandom.Next(_firstnames.Count)];
                string LastName = _lastnames[userRandom.Next(_lastnames.Count)];
                string username = $"{FirstName.ToLower().Replace(" ", "")}-{LastName.ToLower().Replace(" ", "")}{x}";
                string email = $"{username.Replace("-", ".")}@example.com".Replace(" ", "");
                string githubUsername = username;
                int phonenum = userRandom.Next(10) % 2 == 0 ? userRandom.Next(40000000, 49999999) : userRandom.Next(40000000, 49999999);
                string mobile = $"+47{phonenum}";
                string specialism = _specialisms[userRandom.Next(_specialisms.Count)];
                string bio = $"Hi, I'm {FirstName} and I specialize in {specialism.Split(" ")[0]}.";
                string password = _passwordHashes[userRandom.Next(_passwordHashes.Count)];
                Roles role = (Roles)(userRandom.Next(2)); // 0 = teacher, 1 = student
                string photo = (x - 1 < allPhotos.Count) ? allPhotos[x - 1] : $"https://i.pravatar.cc/150?u={email}";

                User user = new User
                {
                    Id = x,
                    FirstName = FirstName,
                    LastName = LastName,
                    Username = username,
                    Email = email,
                    GithubUsername = githubUsername,
                    Mobile = mobile,
                    Bio = bio,
                    Specialism = specialism,
                    Role = role,
                    PasswordHash = password,
                    StartDate = somedate.AddDays(-userRandom.Next(30, 30)),
                    EndDate = somedate.AddDays(userRandom.Next(31, 365)),
                    Photo = photo
                };

                _users.Add(user);
            }
        }

        public List<User> Users { get { return _users; } }
    }
}

