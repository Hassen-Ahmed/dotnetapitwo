using System.Data;
using DotnetApi.Models;
using DotnetApi.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Data;

public class DataContextEF : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserSalary> UserSalary { get; set; }
    public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }
    public virtual DbSet<Auth> Auth { get; set; }
    public virtual DbSet<Post> Posts { get; set; }

    private readonly IConfiguration _config;
    public DataContextEF(IConfiguration config)
    {
        _config = config;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("TutorialAppSchema");

        modelBuilder.Entity<User>(ur =>
        {
            ur.ToTable("Users", "TutorialAppSchema");
            ur.HasKey(u => u.UserId);
        });

        modelBuilder.Entity<UserSalary>(us =>
        {
            us.HasKey(u => u.UserId);
        });

        modelBuilder.Entity<UserJobInfo>(uji =>
        {
            uji.HasKey(u => u.UserId);
        });

        modelBuilder.Entity<Auth>(a =>
        {
            a.HasKey(u => u.Email);
        });

        modelBuilder.Entity<Post>(p =>
        {
            p.HasKey(u => u.PostId);
        });

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
            optionsBuilder => optionsBuilder.EnableRetryOnFailure());
        }

    }
}

/*
   public partial class ContosoPizzaContext : DbContext
   {
       public ContosoPizzaContext()
       {
       }

       public ContosoPizzaContext(DbContextOptions<ContosoPizzaContext> options)
           : base(options)
       {
       }
       public virtual DbSet<Customer> Customers { get; set; } = null!;
       public virtual DbSet<Order> Orders { get; set; } = null!;
       public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
       public virtual DbSet<Product> Products { get; set; } = null!;

       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       {
           if (!optionsBuilder.IsConfigured)
           {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
               optionsBuilder.UseSqlServer("Data Source=localhost;Database=ContosoPizza;Integrated Security=false;User ID=sa;Password=P@ssw0rd;");
           }
       }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           modelBuilder.Entity<Order>(entity =>
           {
               entity.HasIndex(e => e.CustomerId, "IX_Orders_CustomerId");

               entity.HasOne(d => d.Customer)
                   .WithMany(p => p.Orders)
                   .HasForeignKey(d => d.CustomerId);
           });

           modelBuilder.Entity<OrderDetail>(entity =>
           {
               entity.HasIndex(e => e.OrderId, "IX_OrderDetails_OrderId");

               entity.HasIndex(e => e.ProductId, "IX_OrderDetails_ProductId");

               entity.HasOne(d => d.Order)
                   .WithMany(p => p.OrderDetails)
                   .HasForeignKey(d => d.OrderId);

               entity.HasOne(d => d.Product)
                   .WithMany(p => p.OrderDetails)
                   .HasForeignKey(d => d.ProductId);
           });

           modelBuilder.Entity<Product>(entity =>
           {
               entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");
           });

           OnModelCreatingPartial(modelBuilder);
       }

       partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
   }
   */