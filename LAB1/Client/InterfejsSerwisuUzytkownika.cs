namespace LAB1.Client
{
    public interface InterfejsSerwisuUzytkownika
    {
        Task Login(UzytkownikWprowadzony theuser);
        Task Rejestracja(UzytkownikWprowadzony theuser);
        Task PobierzLogin();
        Task PobierzRole();
    }
}