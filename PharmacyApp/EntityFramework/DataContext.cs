using Microsoft.EntityFrameworkCore;
using PharmacyApp.Models;

namespace PharmacyApp.EntityFramework
{
    /// <summary>
    /// The single EF Core DbContext for the whole application (per rulebook Rule 7.1).
    /// Every DAL method instantiates a fresh instance of this class inside a using block —
    /// never a shared/injected instance (Rule 7.4 forbidden behavior #4).
    ///
    /// DbSet&lt;T&gt; properties and their OnModelCreating configuration are added here as
    /// each entity module is built (Step 2 onward), per Section 11 checklist item 2.
    /// </summary>
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=PharmacyManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MedicineCategory>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<MedicineCategory>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Manufacturer>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<Manufacturer>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Supplier>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<Supplier>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Medicine>()
    .HasKey(e => e.Id);
            modelBuilder.Entity<Medicine>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Medicine>()
                .HasOne(e => e.MedicineCategory)
                .WithMany()
                .HasForeignKey(e => e.MedicineCategoryId);
            modelBuilder.Entity<Medicine>()
                .HasOne(e => e.Manufacturer)
                .WithMany()
                .HasForeignKey(e => e.ManufacturerId);
            modelBuilder.Entity<Medicine>()
                .Property(e => e.RequiresPrescription)
                .HasDefaultValue(false);
            modelBuilder.Entity<Medicine>()
                .Property(e => e.IsControlledOrRestricted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductVariant>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<ProductVariant>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<ProductVariant>()
                .HasOne(e => e.Medicine)
                .WithMany()
                .HasForeignKey(e => e.MedicineId);
            modelBuilder.Entity<ProductVariant>()
                .Property(e => e.UnitsPerPackage)
                .HasDefaultValue(1);
            modelBuilder.Entity<ProductVariant>()
                .Property(e => e.SellingPrice)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Batch>()
    .HasKey(e => e.Id);
            modelBuilder.Entity<Batch>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Batch>()
                .HasOne(e => e.ProductVariant)
                .WithMany()
                .HasForeignKey(e => e.ProductVariantId);
            modelBuilder.Entity<Batch>()
                .Property(e => e.PurchaseCost)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Batch>()
                .Ignore(e => e.ExpiryStatus);

            modelBuilder.Entity<StockTransaction>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<StockTransaction>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<StockTransaction>()
                .HasOne(e => e.ProductVariant)
                .WithMany()
                .HasForeignKey(e => e.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StockTransaction>()
                .HasOne(e => e.Batch)
                .WithMany()
                .HasForeignKey(e => e.BatchId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Purchase>()
    .HasKey(e => e.Id);
            modelBuilder.Entity<Purchase>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Purchase>()
                .HasOne(e => e.Supplier)
                .WithMany()
                .HasForeignKey(e => e.SupplierId);
            modelBuilder.Entity<Purchase>()
                .Ignore(e => e.IsDraft);

            modelBuilder.Entity<PurchaseItem>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<PurchaseItem>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(e => e.ProductVariant)
                .WithMany()
                .HasForeignKey(e => e.ProductVariantId);
            modelBuilder.Entity<PurchaseItem>()
                .Property(e => e.PurchaseCost)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PurchaseItem>()
                .Ignore(e => e.LineTotal);

            modelBuilder.Entity<Sale>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<Sale>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Sale>()
                .Property(e => e.DiscountAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Sale>()
                .Ignore(e => e.TotalAmount);

            modelBuilder.Entity<SaleItem>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<SaleItem>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SaleItem>()
                .HasOne(e => e.ProductVariant)
                .WithMany()
                .HasForeignKey(e => e.ProductVariantId);
            modelBuilder.Entity<SaleItem>()
                .Property(e => e.UnitPriceAtSale)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SaleItem>()
                .Ignore(e => e.LineTotal);

            modelBuilder.Entity<Payment>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<Payment>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Payment>()
                .Property(e => e.AmountPaid)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<User>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<User>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<User>()
                .Property(e => e.IsActive)
                .HasDefaultValue(true);
        }

        public DbSet<MedicineCategory> MedicineCategories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }

        // ... remaining entities added in Steps 3-8
    }
}
