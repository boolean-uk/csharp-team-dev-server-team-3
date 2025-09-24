using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace exercise.tests.IntegrationTests
{
    public class TokenTests : BaseIntegrationTest
    {
        [Test]
        public async Task CreateToken_ShouldGenerateValidJwt()
        {
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);


            //var realid = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            string? email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string? role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;


            using (Assert.EnterMultipleScope())
            {
                Assert.That(email, Is.EqualTo(TeacherEmail));
                Assert.That(role, Is.EqualTo("1"));
                Assert.That(jwt.ValidTo, Is.GreaterThan(DateTime.UtcNow));
            }
        }

        [Test]
        public async Task CreateToken_LongLife_ShouldExpireLater()
        {
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword, true, true);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            //var realid = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            string? email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string? role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            //Console.WriteLine(expClaim);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(email, Is.EqualTo(TeacherEmail));
                Assert.That(role, Is.EqualTo("1"));
                Assert.That(jwt.ValidTo, Is.GreaterThan(DateTime.UtcNow.AddDays(6.5)));
                Assert.That(jwt.ValidTo, Is.LessThan(DateTime.UtcNow.AddDays(7.5)));
            }
        }

        [Test]
        public async Task CreateToken_NormalLife_ShouldExpireLater()
        {
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            //var realid = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            string? email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string? role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            //Console.WriteLine(expClaim);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(email, Is.EqualTo(TeacherEmail));
                Assert.That(role, Is.EqualTo("1"));
                Assert.That(jwt.ValidTo, Is.GreaterThan(DateTime.UtcNow.AddMinutes(50)));
                Assert.That(jwt.ValidTo, Is.LessThan(DateTime.UtcNow.AddHours(2)));
            }
        }

    }
}
