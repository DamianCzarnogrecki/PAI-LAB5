using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace LAB1.Client
{
    public class SerwisPanstwa : InterfejsSerwisuPanstwa
    {
        private HttpClient httpClient;
        private NavigationManager navigationManager;

        public SerwisPanstwa(HttpClient httpClient, NavigationManager navigationManager)
        {
            this.httpClient = httpClient;
            this.navigationManager = navigationManager;
        }

        public List<Panstwo> Panstwa { get; set; } = new List<Panstwo>();
        public List<TypRzadu> TypyRzadu { get; set; } = new List<TypRzadu>();

        private async Task PobierzPanstwa(HttpResponseMessage wynik)
        {
            var odpowiedz = await wynik.Content.ReadFromJsonAsync<List<Panstwo>>();
            Panstwa = odpowiedz;
            navigationManager.NavigateTo("/");
        }

        public async Task UtworzPanstwo(Panstwo panstwo)
        {
            var wynik = await httpClient.PostAsJsonAsync("api", panstwo);
            await PobierzPanstwa(wynik);
        }

        public async Task UsunPanstwo(int id)
        {
            var wynik = await httpClient.DeleteAsync($"api/{id}");
            await PobierzPanstwa(wynik);
        }

        public async Task OdczytajTypRzadu()
        {
            var wynik = await httpClient.GetFromJsonAsync<List<TypRzadu>>("api/typRzadu");
            TypyRzadu = wynik;
        }

        public async Task<Panstwo> WyswietlPanstwo(int id)
        {
            var wynik = await httpClient.GetFromJsonAsync<Panstwo>($"api/{id}");
            return wynik;
            throw new Exception("PAŃSTWA NIE ZNALEZIONO");
        }

        public async Task WyswietlPanstwa()
        {
            var wynik = await httpClient.GetFromJsonAsync<List<Panstwo>>("api");
            Panstwa = wynik;
        }

        public async Task ZmodyfikujPanstwo(Panstwo panstwo)
        {
            var wynik = await httpClient.PutAsJsonAsync($"api/{panstwo.ID}", panstwo);
            await PobierzPanstwa(wynik);
        }
    }
}