using exercise.tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace exercise.tests.IntegrationTests
{
    internal class CohortTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            // Arrange 
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task GetAllCohorts()
        {
            var response = await _client.GetAsync("/cohorts");
            var contentString = await response.Content.ReadAsStringAsync();
            var message = string.IsNullOrWhiteSpace(contentString) ? null : JsonNode.Parse(contentString);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(message, Is.Not.Null);
            //Assert.That(message?["message"]?.GetValue<string>(), Is.EqualTo("success"));

            var data = message?["data"]?.AsArray();
            Assert.That(data, Is.Not.Null);
            Assert.That(data!.Count, Is.GreaterThan(0));

            var cohort = data!.First();
            Assert.That(cohort["id"]?.GetValue<int>(), Is.GreaterThan(0));
            Assert.That(cohort["title"].GetValue<string>, Is.Not.Null);
            //Assert.That(cohort["courses"]["students"].GetValue<Array>, Is.Not.Null);
        }
    }
}
