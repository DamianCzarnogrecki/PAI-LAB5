namespace LAB1.Server
{
    public class Kontekst : DbContext
    {
        public Kontekst(DbContextOptions<Kontekst> options) : base(options) { }
        public DbSet<Panstwo>? Panstwo { get; set; }
        public DbSet<TypRzadu>? TypRzadu { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<Roles>? Roles { get; set; }
        public DbSet<UserRole>? UserRole { get; set; }
        public DbSet<PlikOdUzytkownika> Wrzuc => Set<PlikOdUzytkownika>();
    }
}