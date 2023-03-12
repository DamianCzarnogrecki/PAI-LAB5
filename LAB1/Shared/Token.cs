namespace LAB1.Shared
{
    public static class Token
    {
        public static string JWT { get; set; } = "";
        public static string Login { get; set; } = "";
        public static List<string> Roles { get; set; } = new List<string> { };
    }
}