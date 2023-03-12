using System.Net.Http.Json;
using Newtonsoft.Json;

namespace LAB1.Client
{
    public class SerwisUzytkownika : InterfejsSerwisuUzytkownika
    {
        static HttpClient client = new HttpClient();

        public async Task Login(UzytkownikWprowadzony uzytkownik)
        {
            var zwrocone = await client.PostAsJsonAsync("https://localhost:7121/api/login", uzytkownik);
            string wynik = await zwrocone.Content.ReadAsStringAsync();
            Token.JWT = wynik;
        }

        public async Task Rejestracja(UzytkownikWprowadzony uzytkownik)
        {
            var zwrocone = await client.PostAsJsonAsync("https://localhost:7121/api/register", uzytkownik);
        }

        public async Task PobierzLogin()
        {
            if (Token.JWT != "")
            {
                var zwrocone = await client.GetAsync("https://localhost:7121/api/pobierzlogin");
                string wynik = await zwrocone.Content.ReadAsStringAsync();
                Token.Login = wynik;
            }
        }

        public async Task PobierzRole()
        {
            if (Token.JWT != "")
            {
                var zwrocone = await client.GetAsync("https://localhost:7121/api/pobierzlogin");
                string wynik = zwrocone.Content.ReadAsStringAsync().Result;
                var lista = JsonConvert.DeserializeObject<List<string>>(wynik);
                if (lista != null) Token.Roles = lista;
                else Token.Roles = new List<string>();
            }
            else Token.Roles = new List<string>();
        }
    }
}