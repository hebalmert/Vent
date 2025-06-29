using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vent.Shared.Entities;
using Vent.Shared.EntitiesSoft;
using Vent.Shared.EntitiesSoftSec;

namespace Vent.AccessData.Data;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    //Manejo de UserRoles por Usuario

    public DbSet<UserRoleDetails> UserRoleDetails => Set<UserRoleDetails>();

    //Entities

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<State> States => Set<State>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<SoftPlan> SoftPlans => Set<SoftPlan>();
    public DbSet<Corporation> Corporations => Set<Corporation>();
    public DbSet<Manager> Managers => Set<Manager>();

    //EntitiesSoftSec

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<UsuarioRole> UsuarioRoles => Set<UsuarioRole>();

    //EntitiesSoft

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tax> Taxes => Set<Tax>();
    public DbSet<Register> Registers => Set<Register>();
    public DbSet<PaymentType> PaymentTypes => Set<PaymentType>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductStorage> ProductStorages => Set<ProductStorage>();
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();
    public DbSet<Sell> Sells => Set<Sell>();
    public DbSet<SellDetails> SellDetails => Set<SellDetails>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<TransferDetails> TransferDetails => Set<TransferDetails>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Para tomar los calores de ConfigEntities
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}