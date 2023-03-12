using LAB1.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LAB1.Server.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class KontrolerUzytkownika : ControllerBase
    {
        private Kontekst kontekst;

        public KontrolerUzytkownika(Kontekst kontekst)
        {
            this.kontekst = kontekst;
        }

        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<List<User>>> WyswietlUzytkownikow()
        {
            var uzytkownicy = from u in kontekst.Users
                              join ur in kontekst.UserRole on u.UserId equals ur.UserId
                              join r in kontekst.Roles on ur.RoleId equals r.RoleId
                              select new { Users = u, Roles = r.RoleName };

            return Ok(uzytkownicy);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<List<User>>> WyswietlUzytkownika(int id)
        {
            var uzytkownik = from u in kontekst.Users
                              where u.UserId == id
                              join ur in kontekst.UserRole on u.UserId equals ur.UserId
                              join r in kontekst.Roles on ur.RoleId equals r.RoleId
                              select new { Users = u, Roles = r.RoleName };

            return Ok(uzytkownik);
        }
    }
}