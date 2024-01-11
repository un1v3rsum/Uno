using System.Data.Entity;
using Domain;

namespace DAL;

public class AppDbContext: DbContext
{
    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<History> Histories { get; set; } = default!;

}