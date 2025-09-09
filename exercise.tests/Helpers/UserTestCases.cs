using NUnit.Framework;
using System.Collections.Generic;

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
            yield return new TestCaseData("ThisIsWayTooLong123ThisIsWayTooLong123ThisIsWayTooLong123", "valid@email.com", "ValidPass1!").SetName("Invalid: Username too long");
            yield return new TestCaseData("", "valid@email.com", "ValidPass1!").SetName("Invalid: Username too short");
            yield return new TestCaseData("validuser", "plainaddress", "ValidPass1!").SetName("Invalid: Invalid email format");
            yield return new TestCaseData("validuser", "user@domain.c", "ValidPass1!").SetName("Invalid: Invalid email domain");
            yield return new TestCaseData("validuser", "valid@email.com", "short1!").SetName("Invalid: Password too short");
            yield return new TestCaseData("validuser", "valid@email.com", "alllowercase1!").SetName("Invalid: Missing uppercase");
            yield return new TestCaseData("validuser", "valid@email.com", "NoNumber!").SetName("Invalid: Missing number");
            yield return new TestCaseData("validuser", "valid@email.com", "NoSpecialChar1").SetName("Invalid: Missing special character");
        }

        public static IEnumerable<TestCaseData> ValidLoginCases()
        {
            yield return new TestCaseData("user1@example.com", "SecurePass9#");
            yield return new TestCaseData("user2@example.com", "StrongPass$1");
            yield return new TestCaseData("valid@email.com", "ValidPass1!");
        }

        public static IEnumerable<TestCaseData> InvalidLoginCases()
        {
            yield return new TestCaseData("valid@email.com", "short1!").SetName("Invalid: Password too short");
            yield return new TestCaseData("valid@email.com", "alllowercase1!").SetName("Invalid: Missing uppercase");
            yield return new TestCaseData("valid@email.com", "NoNumber!").SetName("Invalid: Missing number");
            yield return new TestCaseData("valid@email.com", "NoSpecialChar1").SetName("Invalid: Missing special character");
            yield return new TestCaseData("plainaddress", "ValidPass1!").SetName("Invalid: Invalid email format");
            yield return new TestCaseData("user@domain.c", "ValidPass1!").SetName("Invalid: Invalid email domain");
        }
    }
}