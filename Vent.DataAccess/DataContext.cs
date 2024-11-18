using Microsoft.EntityFrameworkCore;
using Vent.Shared.Entities;

namespace Vent.DataAccess;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();

    public DbSet<State> States => Set<State>();

    public DbSet<City> Cities => Set<City>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Validaciones
        modelBuilder.Entity<Country>().HasIndex(e => e.Name).IsUnique();

        modelBuilder.Entity<State>().HasIndex(e => new { e.Name, e.CountryId }).IsUnique();

        modelBuilder.Entity<City>().HasIndex(e => new { e.StateId, e.Name }).IsUnique();

        DisableCascadingDelete(modelBuilder);
    }

    private void DisableCascadingDelete(ModelBuilder modelBuilder)
    {
        var relationShips = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
        foreach (var item in relationShips)
        {
            item.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}