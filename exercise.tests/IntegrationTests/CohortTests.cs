using exercise.tests.Helpers;
using exercise.wwwapi.DTOs.Cohort;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace exercise.tests.IntegrationTests
{
    /// <summary>
    /// Integration tests covering the lifecycle of posts via the public API endpoints.
    /// </summary>
    [TestFixture]
    public class CohortTests : BaseIntegrationTest
    {
        [TestCase(TeacherEmail, TeacherPassword, "Success", HttpStatusCode.OK)]
        [TestCase(StudentEmail1, StudentPassword1, "You are not authorized to get all cohorts.", HttpStatusCode.Forbidden)]
        public async Task GetAllCohorts(string email, string password, string content, HttpStatusCode expected)
        {
            string token = await LoginAndGetToken(email, password);
            HttpResponseMessage response = await SendAuthenticatedGetAsync($"/cohorts", token);
            JsonNode? message = await response.ReadJsonAsync();

            if (expected == HttpStatusCode.Forbidden)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(content));
                }
            }
                
            if (expected == HttpStatusCode.OK)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(content));
                }
                var data = message?["data"]?.AsArray();
                Assert.That(data, Is.Not.Null);
                Assert.That(data?.Count, Is.GreaterThan(0));

                var cohort = data?.First();
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(cohort?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                    Assert.That(cohort?["title"]?.GetValue<string>(), Is.Not.Null);
                }
                var courses = cohort["courses"]?.AsArray();
                Assert.That(courses, Is.Not.Null);
                Assert.That(courses?.Count, Is.GreaterThan(0));

                var course = courses.First();
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(course?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                    Assert.That(course?["title"]?.GetValue<string>(), Is.Not.Null);
                }

                var students = course["students"]?.AsArray();
                Assert.That(students, Is.Not.Null);
                Assert.That(students?.Count, Is.GreaterThan(0));
                var student = students.First();
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(student?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                    Assert.That(student?["firstName"]?.GetValue<string>(), Is.Not.Null);
                    Assert.That(student?["lastName"]?.GetValue<string>(), Is.Not.Null);
                }

                var teachers = course["teachers"]?.AsArray();
                Assert.That(teachers, Is.Not.Null);
                Assert.That(teachers?.Count, Is.GreaterThan(0));
                var teacher = teachers.First();
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(teacher?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                    Assert.That(teacher?["firstName"]?.GetValue<string>(), Is.Not.Null);
                    Assert.That(teacher?["lastName"]?.GetValue<string>(), Is.Not.Null);
                }
            }
            
        }

        [TestCase(TeacherEmail, TeacherPassword, "Success", HttpStatusCode.OK)]
        [TestCase(StudentEmail1, StudentPassword1, "Success", HttpStatusCode.OK)]
        public async Task GetCohortById(string email, string password, string content, HttpStatusCode expected)
        {
            string token = await LoginAndGetToken(email, password);
            HttpResponseMessage response = await SendAuthenticatedGetAsync($"/cohorts/cohortId/1", token);
            JsonNode? message = await response.ReadJsonAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(expected));
                Assert.That(message, Is.Not.Null);
                Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(content));
            }
            var data = message?["data"]; 
            Assert.That(data, Is.Not.Null);
;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(data?["id"]?.GetValue<int>(), Is.EqualTo(1));
                Assert.That(data?["title"]?.GetValue<string>(), Is.Not.Null);
            }
            var courses = data["courses"]?.AsArray();
            Assert.That(courses, Is.Not.Null);
            Assert.That(courses?.Count, Is.GreaterThan(0));

            var course = courses.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(course?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(course?["title"]?.GetValue<string>(), Is.Not.Null);
            }

            var students = course["students"]?.AsArray();
            Assert.That(students, Is.Not.Null);
            Assert.That(students?.Count, Is.GreaterThan(0));
            var student = students.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(student?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(student?["firstName"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(student?["lastName"]?.GetValue<string>(), Is.Not.Null);
            }

            var teachers = course["teachers"]?.AsArray();
            Assert.That(teachers, Is.Not.Null);
            Assert.That(teachers?.Count, Is.GreaterThan(0));
            var teacher = teachers.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(teacher?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(teacher?["firstName"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(teacher?["lastName"]?.GetValue<string>(), Is.Not.Null);
            }
        }

        [TestCase(TeacherEmail, TeacherPassword, TeacherId, "Success", HttpStatusCode.OK)]
        [TestCase(StudentEmail1, StudentPassword1, StudentId1, "Success", HttpStatusCode.OK)]
        public async Task GetCohortByUserId(string email, string password, int id, string content, HttpStatusCode expected)
        {
            string token = await LoginAndGetToken(email, password);
            HttpResponseMessage response = await SendAuthenticatedGetAsync($"/cohorts/userId/{id}", token);
            JsonNode? message = await response.ReadJsonAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message, Is.Not.Null);
                Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));
            }
            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data?.Count, Is.GreaterThan(0));

            var cohort = data?.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(cohort?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(cohort?["title"]?.GetValue<string>(), Is.Not.Null);
            }
            var courses = cohort["courses"]?.AsArray();
            Assert.That(courses, Is.Not.Null);
            Assert.That(courses?.Count, Is.GreaterThan(0));

            var course = courses.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(course?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(course?["title"]?.GetValue<string>(), Is.Not.Null);
            }

            var students = course["students"]?.AsArray();
            Assert.That(students, Is.Not.Null);
            Assert.That(students?.Count, Is.GreaterThan(0));
            var student = students.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(student?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(student?["firstName"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(student?["lastName"]?.GetValue<string>(), Is.Not.Null);
            }

            var teachers = course["teachers"]?.AsArray();
            Assert.That(teachers, Is.Not.Null);
            Assert.That(teachers?.Count, Is.GreaterThan(0));
            var teacher = teachers.First();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(teacher?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                Assert.That(teacher?["firstName"]?.GetValue<string>(), Is.Not.Null);
                Assert.That(teacher?["lastName"]?.GetValue<string>(), Is.Not.Null);
            }
        }


        [TestCase(TeacherEmail, TeacherPassword, "Test Cohort Teacher", "Success", HttpStatusCode.Created)]
        [TestCase(StudentEmail1, StudentPassword1, "Test Cohort Student", "Success", HttpStatusCode.Forbidden)]
        public async Task CreateCohort(string email, string password, string cohortName, string content, HttpStatusCode expected)
        {
            var uniqueId = DateTime.UtcNow.ToString("yyMMddHHmmssffff");

            string token = await LoginAndGetToken(email, password);
            CreateCohortDTO ccDTO = new CreateCohortDTO() { Title = $"{uniqueId}{cohortName}" };
            HttpResponseMessage response = await SendAuthenticatedPostAsync($"/cohorts", token, ccDTO);
            JsonNode? message = await response.ReadJsonAsync();

            if (expected == HttpStatusCode.Forbidden)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("You are not authorized to create a new cohort."));
                }
            }

            if (expected == HttpStatusCode.Created)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("Success"));
                }

                var data = message?["data"];
                Assert.That(data, Is.Not.Null);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(data?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                    Assert.That(data?["title"]?.GetValue<string>(), Is.EqualTo(ccDTO.Title));
                }

                var courses = data["courses"]?.AsArray();
                Assert.That(courses, Is.Not.Null);
                Assert.That(courses?.Count, Is.EqualTo(3));
                var course = courses.First();
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(course?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                    Assert.That(course?["title"]?.GetValue<string>(), Is.Not.Null);
                }

                var students = course["students"]?.AsArray();
                Assert.That(students, Is.Not.Null);
                Assert.That(students?.Count, Is.EqualTo(0));

                var teachers = course["teachers"]?.AsArray();
                Assert.That(teachers, Is.Not.Null);
                Assert.That(teachers?.Count, Is.EqualTo(0));
            }
        }

        [TestCase(TeacherEmail, TeacherPassword, 1, 1, 1,"Success", HttpStatusCode.OK)]
        [TestCase(TeacherEmail, TeacherPassword, 350, 1, 1, "User with Id 350 not found.", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, 1, 20, 1, "Cohort with Id 20 not found.", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, 1, 1, 10, "The specified course is not part of this cohort.", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, TeacherId, 3, 3, "User is already in the specified course in the cohort.", HttpStatusCode.BadRequest)]
        [TestCase(StudentEmail1, StudentPassword1, 1, 3, 1, "You are not authorized to add a user to a cohort.", HttpStatusCode.Forbidden)]
        public async Task AddUserToCohortAndDeleteUser(
            string email, 
            string password, 
            int userId, 
            int cohortId, 
            int courseId, 
            string content, 
            HttpStatusCode expected)
        {
            string token = await LoginAndGetToken(email, password);
            //HttpResponseMessage deleteUserResponse2 = await SendAuthenticatedDeleteAsync(
            //        $"/cohorts/cohortId/{cohortId}/userId/{userId}/courseId/{courseId}", token);
            HttpResponseMessage response = await SendAuthenticatedPostAsync(
                $"/cohorts/cohortId/{cohortId}/userId/{userId}/courseId/{courseId}", token);
            JsonNode? message = await response.ReadJsonAsync();

            if (expected == HttpStatusCode.BadRequest)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(content));
                }
            }

            if (expected == HttpStatusCode.Forbidden)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(content));
                }
            }

            if (expected == HttpStatusCode.OK)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(content));
                }
                #region Check that user is added
                HttpResponseMessage getResponse = await SendAuthenticatedGetAsync($"/cohorts/cohortId/1", token);
                JsonNode? getMessage = await getResponse.ReadJsonAsync();

                var getData = getMessage?["data"];
                Assert.That(getData, Is.Not.Null);
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(getData?["id"]?.GetValue<int>(), Is.EqualTo(1));
                    Assert.That(getData?["title"]?.GetValue<string>(), Is.Not.Null);
                }
                var getCourses = getData["courses"]?.AsArray();
                Assert.That(getCourses, Is.Not.Null);
                Assert.That(getCourses?.Count, Is.GreaterThan(0));

                var getCourse = getCourses.First();
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(getCourse?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                    Assert.That(getCourse?["title"]?.GetValue<string>(), Is.Not.Null);
                }

                var getTeachers = getCourse["teachers"]?.AsArray();
                Assert.That(getTeachers, Is.Not.Null);
                Assert.That(getTeachers?.Count, Is.GreaterThan(0));
                Assert.That(
                    getTeachers!.Any(user =>
                        user?["firstName"]?.ToString() == "Oyvind" &&
                        user?["lastName"]?.ToString() == "Perez"
                    ),
                    Is.True
                );
                #endregion Check that user is added

                // Delete the user
                HttpResponseMessage deleteUserResponse = await SendAuthenticatedDeleteAsync(
                    $"/cohorts/cohortId/{cohortId}/userId/{userId}/courseId/{courseId}", token);

                #region Check that user is deleted
                HttpResponseMessage deleteResponse = await SendAuthenticatedGetAsync($"/cohorts/cohortId/1", token);
                JsonNode? deleteMessage = await deleteResponse.ReadJsonAsync();

                var deleteData = deleteMessage?["data"];
                Assert.That(deleteData, Is.Not.Null);
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(deleteData?["id"]?.GetValue<int>(), Is.EqualTo(1));
                    Assert.That(deleteData?["title"]?.GetValue<string>(), Is.Not.Null);
                }
                var deleteCourses = deleteData["courses"]?.AsArray();
                Assert.That(deleteCourses, Is.Not.Null);
                Assert.That(deleteCourses?.Count, Is.GreaterThan(0));

                var deleteCourse = deleteCourses.First();
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(deleteCourse?["id"]?.GetValue<int>(), Is.GreaterThan(0));
                    Assert.That(deleteCourse?["title"]?.GetValue<string>(), Is.Not.Null);
                }

                var deleteTeachers = deleteCourse["teachers"]?.AsArray();
                Assert.That(deleteTeachers, Is.Not.Null);
                Assert.That(deleteTeachers?.Count, Is.GreaterThan(0));
                Assert.That(
                    deleteTeachers!.Any(user =>
                        user?["firstName"]?.ToString() == "Oyvind" &&
                        user?["lastName"]?.ToString() == "Perez"
                    ),
                    Is.False
                );
                #endregion Check that user is deleted
            }

        }

        [TestCase(TeacherEmail, TeacherPassword, 350, 1, 1, "User with Id 350 not found.", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, 1, 20, 1, "Cohort with Id 20 not found.", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, 11, 1, 10, "The specified course is not part of this cohort.", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, 1, 1, 1, "The specified user is not part of this cohort.", HttpStatusCode.BadRequest)]
        [TestCase(TeacherEmail, TeacherPassword, 11, 1, 2, "User is in cohort, but is not taking the specified course.", HttpStatusCode.BadRequest)]
        [TestCase(StudentEmail1, StudentPassword1, 1, 1, 1, "You are not authorized to delete a user from a cohort.", HttpStatusCode.Forbidden)]
        public async Task InvalidDeletes(
            string email,
            string password,
            int userId,
            int cohortId,
            int courseId,
            string content,
            HttpStatusCode expected)
        {

            string token = await LoginAndGetToken(email, password);
            HttpResponseMessage response = await SendAuthenticatedDeleteAsync(
                    $"/cohorts/cohortId/{cohortId}/userId/{userId}/courseId/{courseId}", token);
            JsonNode? message = await response.ReadJsonAsync();

            if (expected == HttpStatusCode.BadRequest)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(content));
                }
            }

            if (expected == HttpStatusCode.Forbidden)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected));
                    Assert.That(message, Is.Not.Null);
                    Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo(content));
                }
            }
        }
    }
}