namespace exercise.tests.Helpers
{
    public static class UserTestCases
    {
        public static IEnumerable<TestCaseData> ValidRegisterCases()
        {
            yield return new TestCaseData("validuser", "valid@email.com", "ValidPass1!");
            yield return new TestCaseData("user-name", "user1@example.com", "SecurePass9#");
            yield return new TestCaseData("user1name", "user2@example.com", "StrongPass$1");
        }

        public static IEnumerable<TestCaseData> InvalidRegisterCases()
        {
            yield return new TestCaseData("validuser", "plainaddress", "ValidPass1!").SetName("Invalid: Invalid email format (register)");
            yield return new TestCaseData("validuser", "user@domain.c", "ValidPass1!").SetName("Invalid: Invalid email domain (register)");
            yield return new TestCaseData("validuser", "valid@email.com", "short1!").SetName("Invalid: Password too short (register)");
            yield return new TestCaseData("validuser", "valid@email.com", "alllowercase1!").SetName("Invalid: Missing uppercase (register)");
            yield return new TestCaseData("validuser", "valid@email.com", "NoNumber!").SetName("Invalid: Missing number (register)");
            yield return new TestCaseData("validuser", "valid@email.com", "NoSpecialChar1").SetName("Invalid: Missing special character (register)");
        }

        public static IEnumerable<TestCaseData> ValidLoginCases()
        {
            yield return new TestCaseData("oyvind.perez1@example.com", "SuperHash!4");
            //needs valid email password combinations
            //yield return new TestCaseData("nigel.nowak2@example.com", "StrongPass$1");
            //yield return new TestCaseData("nigel.nowak2@example.com", "ValidPass1!");
        }

        public static IEnumerable<TestCaseData> InvalidLoginCases()
        {
            yield return new TestCaseData("valid@email.com", "short1!").SetName("Invalid: Password too short (login)");
            yield return new TestCaseData("valid@email.com", "alllowercase1!").SetName("Invalid: Missing uppercase (login)");
            yield return new TestCaseData("valid@email.com", "NoNumber!").SetName("Invalid: Missing number (login)");
            yield return new TestCaseData("valid@email.com", "NoSpecialChar1").SetName("Invalid: Missing special character (login)");
            yield return new TestCaseData("plainaddress", "ValidPass1!").SetName("Invalid: Invalid email format (login)");
            yield return new TestCaseData("user@domain.c", "ValidPass1!").SetName("Invalid: Invalid email domain (login)");
        }
    }
}