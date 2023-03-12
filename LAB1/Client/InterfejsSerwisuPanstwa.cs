namespace LAB1.Client
{
    public interface InterfejsSerwisuPanstwa
    {
        List<Panstwo> Panstwa { get; set; }
        List<TypRzadu> TypyRzadu { get; set; }
        Task OdczytajTypRzadu();
        Task WyswietlPanstwa();
        Task<Panstwo> WyswietlPanstwo(int id);
        Task UtworzPanstwo(Panstwo panstwo);
        Task ZmodyfikujPanstwo(Panstwo panstwo);
        Task UsunPanstwo(int id);
    }
}