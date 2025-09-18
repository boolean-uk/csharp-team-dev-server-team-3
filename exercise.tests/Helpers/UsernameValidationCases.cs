using System.Net;

namespace exercise.tests.Helpers
{
    public static class UsernameValidationTestData
    {
        public static IEnumerable<TestCaseData> UsernameValidationMessageCases()
        {
            string[] endpoints = new string[]
            {
                "username",
                "git-username"
            };

            var cases = new List<(string input, string expected)>
            {
                ("username", "Accepted"),
                ("user-name", "Accepted"),
                ("user-name-valid", "Accepted"),
                ("username1", "Accepted"),
                ("user1name", "Accepted"),
                ("1user-name", "Accepted"),
                ("user-name-1", "Accepted"),
                ("u", "Accepted"),
                ("1", "Accepted"),
                ("usernameusernameusernameusernameusernameusername", "Length of username must be at most 39."),
                ("-username", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username-", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("-username-", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("-", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("user--name", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("user--name-invalid", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username!", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username?", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username+", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username`", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("user.name", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username´", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username/", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username%", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("username_", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("user name", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
                ("øsername", "Username must contain only alphanumeric characters that may be separated by single hyphens"),
            };

            foreach (var endpoint in endpoints)
            {
                foreach (var test in cases)
                {
                    yield return new TestCaseData(endpoint, test.input, test.expected)
                        .SetName($"ValidateUsername('{test.input}') on {endpoint}");
                }
            }

        }
        public static IEnumerable<TestCaseData> UsernameValidationStatusCases()
        {
            string[] endpoints = new string[]
            {
                "username",
                "git-username"
            };

            var cases = new List<(string input, HttpStatusCode expected)>
            {
                ("username", HttpStatusCode.OK),
                ("user-name", HttpStatusCode.OK),
                ("user-name-valid", HttpStatusCode.OK),
                ("username1", HttpStatusCode.OK),
                ("user1name", HttpStatusCode.OK),
                ("1user-name", HttpStatusCode.OK),
                ("user-name-1", HttpStatusCode.OK),
                ("u", HttpStatusCode.OK),
                ("1", HttpStatusCode.OK),
                ("usernameusernameusernameusernameusernameusername", HttpStatusCode.BadRequest),
                ("-username",HttpStatusCode.BadRequest),
                ("username-", HttpStatusCode.BadRequest),
                ("-username-", HttpStatusCode.BadRequest),
                ("-", HttpStatusCode.BadRequest),
                ("user--name", HttpStatusCode.BadRequest),
                ("user--name-invalid", HttpStatusCode.BadRequest),
                ("username!", HttpStatusCode.BadRequest),
                ("username?", HttpStatusCode.BadRequest),
                ("username+", HttpStatusCode.BadRequest),
                ("username`", HttpStatusCode.BadRequest),
                ("username´", HttpStatusCode.BadRequest),
                ("username/", HttpStatusCode.BadRequest),
                ("username%", HttpStatusCode.BadRequest),
                ("username_", HttpStatusCode.BadRequest),
                ("user name", HttpStatusCode.BadRequest),
                ("øsername", HttpStatusCode.BadRequest),
            };

            foreach (var endpoint in endpoints)
            {
                foreach (var test in cases)
                {
                    yield return new TestCaseData(endpoint, test.input, test.expected)
                        .SetName($"ValidateUsername('{test.input}') on {endpoint}");
                }
            }
        }

    }
}
