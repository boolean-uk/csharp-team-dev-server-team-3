using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

            Console.WriteLine(jwt);
        }

        [Test]
        public async Task CreateToken_LongLife_ShouldExpireLater() {
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword, true, true);
        }

        [Test]
        public async Task CreateToken_NormalLife_ShouldExpireLater()
        {
            string token = await LoginAndGetToken(TeacherEmail, TeacherPassword, true, true);
        }

    }
}
