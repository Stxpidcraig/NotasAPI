using Microsoft.EntityFrameworkCore;

class NotaDb : DbContext
{
    public NotaDb(DbContextOptions<NotaDb> options)
        : base(options) { }

    public DbSet<Nota> Notas => Set<Nota>();
}