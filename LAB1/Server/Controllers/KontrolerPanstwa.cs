using Microsoft.AspNetCore.Mvc;

namespace LAB1.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class KontrolerPanstwa : ControllerBase
    {
        private Kontekst kontekst;

        public KontrolerPanstwa(Kontekst kontekst)
        {
            this.kontekst = kontekst;
        }

        [HttpGet("typRzadu")]
        public async Task<ActionResult<List<TypRzadu>>> OdczytajTypRzadu()
        {
            var TypyRzadu = await kontekst.TypRzadu.ToListAsync();
            return Ok(TypyRzadu);
        }

        [HttpGet]
        public async Task<ActionResult<List<Panstwo>>> WyswietlPanstwa()
        {
            var panstwa = await kontekst.Panstwo.Include(panstwo => panstwo.TypRzadu).ToListAsync();
            return Ok(panstwa);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Panstwo>> WyswietlPanstwo(int id)
        {
            var panstwo = await kontekst.Panstwo.Include(panstwo => panstwo.TypRzadu).FirstOrDefaultAsync(panstwo => panstwo.ID == id);

            if (panstwo == null)
            {
                return NotFound("PAŃSTWA NIE ZNALEZIONO");
            }

            return Ok(panstwo);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Panstwo>>> UsunPanstwo(int id)
        {
            var panstwoBazy = await kontekst.Panstwo.Include(panstwo => panstwo.TypRzadu).FirstOrDefaultAsync(panstwo => panstwo.ID == id);

            if (panstwoBazy == null)
            {
                return NotFound("PAŃSTWA NIE ZNALEZIONO");
            }

            kontekst.Panstwo.Remove(panstwoBazy);
            await kontekst.SaveChangesAsync();
            return Ok(await PobierzPanstwa());
        }

        [HttpPost]
        public async Task<ActionResult<List<Panstwo>>> UtworzPanstwo(Panstwo panstwo)
        {
            panstwo.TypRzadu = null;
            kontekst.Panstwo.Add(panstwo);
            await kontekst.SaveChangesAsync();
            return Ok(await PobierzPanstwa());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Panstwo>>> ZmodyfikujPanstwo(Panstwo panstwo, int id)
        {
            var panstwoBazy = await kontekst.Panstwo.Include(panstwo => panstwo.TypRzadu).FirstOrDefaultAsync(panstwo => panstwo.ID == id);

            if (panstwoBazy == null)
            {
                return NotFound("PAŃSTWA NIE ZNALEZIONO");
            }

            panstwoBazy.ID = panstwo.ID;
            panstwoBazy.NazwaPowszechna = panstwo.NazwaPowszechna;
            panstwoBazy.NazwaOficjalna = panstwo.NazwaOficjalna;
            panstwoBazy.KodISO3166 = panstwo.KodISO3166;
            panstwoBazy.TypRzaduID = panstwo.TypRzaduID;
            panstwoBazy.LiczbaObywateli = panstwo.LiczbaObywateli;
            panstwoBazy.HDI = panstwo.HDI;
            panstwoBazy.URLflagi = panstwo.URLflagi;

            await kontekst.SaveChangesAsync();

            return Ok(await PobierzPanstwa());
        }

        private async Task<List<Panstwo>> PobierzPanstwa()
        {
            return await kontekst.Panstwo.Include(panstwo => panstwo.TypRzadu).ToListAsync();
        }
    }
}