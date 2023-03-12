using LAB1.Client;
using LAB1.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LAB1.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class KontrolerLogowania : ControllerBase
    {
        private readonly IConfiguration configuration;
        public readonly InterfejsSerwisuUzytkownika serwisUzytkownika;
        private Kontekst kontekst;

        public KontrolerLogowania(
            IConfiguration configuration,
            InterfejsSerwisuUzytkownika serwisUzytkownika,
            Kontekst kontekst
        )
        {
            this.configuration = configuration;
            this.serwisUzytkownika = serwisUzytkownika;
            this.kontekst = kontekst;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Rejestracja(UzytkownikWprowadzony uzytkownik)
        {
            var uzytkownikMaxID = kontekst.Users?.OrderByDescending(u => u.UserId).FirstOrDefault();
            User user = new User();
            user.UserId = uzytkownikMaxID?.UserId + 1;
            user.Login = uzytkownik.Login;
            user.AddTime = DateTime.Now;
            Hashowanie(uzytkownik.Haslo, out byte[] hash, out byte[] salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            kontekst.Users?.Add(user);
            await kontekst.SaveChangesAsync();
            return Ok(user);
        }

        [HttpGet("pobierzlogin")]
        public string? PobierzLogin()
        {
            var jwt = Token.JWT;
            var handler = new JwtSecurityTokenHandler();
            var claim = handler
                .ReadJwtToken(jwt)
                .Claims.FirstOrDefault(x => x.Type.ToString().Equals("name"));
            return claim?.ToString();
        }

        [HttpGet("pobierzrole")]
        public List<string> PobierzRole()
        {
            var jwt = Token.JWT;
            var handler = new JwtSecurityTokenHandler();
            var claim = handler
                .ReadJwtToken(jwt)
                .Claims.Where(x => x.Type.ToString().Equals("role"));
            var lista = claim.ToList().Select(i => i.ToString()).ToList();
            return lista;
        }

        [HttpPost("login")]
        public async Task<string> Login(UzytkownikWprowadzony uzytkownikWprowadzony)
        {
            var uzytkownikBazy = await kontekst.Users.FirstOrDefaultAsync(uzytkownik => uzytkownik.Login == uzytkownikWprowadzony.Login);
            if (uzytkownikBazy == null) return "NIE MA TAKIEGO UŻYTKOWNIKA";

            if (uzytkownikBazy.PasswordHash != null && uzytkownikBazy.PasswordSalt != null)
            {
                if (
                    WeryfikacjaHasla(
                        uzytkownikWprowadzony.Haslo,
                        uzytkownikBazy.PasswordHash,
                        uzytkownikBazy.PasswordSalt
                    )
                )
                {
                    if (kontekst.UserRole != null)
                    {
                        var roleUzytkownika = await kontekst.UserRole
                            .Where(r => r.UserId == uzytkownikBazy.UserId)
                            .ToListAsync();

                        string token = WygenerujToken(uzytkownikBazy, roleUzytkownika);

                        return token;
                    }
                    else return "BŁĄD DOSTĘPU - UŻYTKOWNIK NIE MA PRZYPISANEJ ROLI";
                }
            }
            return "NIEPRAWIDŁOWE HASŁO";
        }

        private string WygenerujToken(User user, List<UserRole> userrole)
        {
            List<Claim> claims;

            if (user.Login != null) claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };
            else claims = new List<Claim> { };

            foreach (var r in userrole) claims.Add(new Claim(ClaimTypes.Role, r.RoleId.ToString()));

            var klucz = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));

            var poswiadczenia = new SigningCredentials(
                klucz,
                SecurityAlgorithms.HmacSha512Signature
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: poswiadczenia
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void Hashowanie(string haslo, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(haslo));
            }
        }

        private bool WeryfikacjaHasla(string haslo, byte[] hash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var gotowyHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(haslo));
                return gotowyHash.SequenceEqual(hash);
            }
        }
    }
}