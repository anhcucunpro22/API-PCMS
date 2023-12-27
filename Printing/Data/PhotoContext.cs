using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Printing.Models;

namespace Printing.Data;

//dotnet ef dbcontext scaffold "Data Source=DESKTOP-CURC7TC\\LEHUY;Initial Catalog=Photo;User Id=sa;Password=123;Trusted_Connection=False;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -f

public partial class PhotoContext : DbContext
{
    public PhotoContext()
    {
    }

    public PhotoContext(DbContextOptions<PhotoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Debt> Debt { get; set; }

    public virtual DbSet<Facilities> Facilities { get; set; }

    public virtual DbSet<InventoryIn> InventoryIn { get; set; }

    public virtual DbSet<InventoryInDetail> InventoryInDetail { get; set; }

    public virtual DbSet<InventoryOut> InventoryOut { get; set; }

    public virtual DbSet<InventoryOutDetail> InventoryOutDetail { get; set; }

    public virtual DbSet<Material> Material { get; set; }

    public virtual DbSet<MaterialGroup> MaterialGroup { get; set; }

    public virtual DbSet<Organizations> Organizations { get; set; }

    public virtual DbSet<Photocopier> Photocopier { get; set; }

    public virtual DbSet<Receipt> Receipt { get; set; }

    public virtual DbSet<ReceiptDetail> ReceiptDetail { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<School> School { get; set; }

    public virtual DbSet<Services> Services { get; set; }

    public virtual DbSet<ServiceGroup> ServiceGroup { get; set; }

    public virtual DbSet<Suppliers> Suppliers { get; set; }

    public virtual DbSet<UnitOfMeasure> UnitOfMeasure { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<UserFacilities> UserFacilities { get; set; }

    public virtual DbSet<UserRole> UserRole { get; set; }

    public virtual DbSet<Warehouses> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-CURC7TC\\LEHUY;Initial Catalog=Photo;User Id=sa;Password=123;Trusted_Connection=False;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Debt>(entity =>
        {
            entity.HasKey(e => e.DebtID).HasName("PK__Debt__5F7687B5039AC7FB");

            entity.ToTable("Debt");

            entity.Property(e => e.DebtID).HasColumnName("DebtID");
            entity.Property(e => e.DebtAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DueDate).HasColumnType("date");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserID).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Debts)
                .HasForeignKey(d => d.UserID)
                .HasConstraintName("FK__Debt__UserID__619B8048");
        });

        modelBuilder.Entity<Facilities>(entity =>
        {
            entity.HasKey(e => e.FacilityID).HasName("PK__Faciliti__5FB08B94CF16AD10");

            entity.Property(e => e.FacilityID).HasColumnName("FacilityID");
            entity.Property(e => e.FacilityName).HasMaxLength(50);
        });

        modelBuilder.Entity<InventoryIn>(entity =>
        {
            entity.HasKey(e => e.InventoryInID).HasName("PK__Inventor__BDF1FDD083D850D0");

            entity.ToTable("InventoryIn");

            entity.Property(e => e.InventoryInID).HasColumnName("InventoryInID");
            entity.Property(e => e.InDate).HasColumnType("date");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SupplierID).HasColumnName("SupplierID");
            entity.Property(e => e.WarehouseID).HasColumnName("WarehouseID");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.AmountReceived).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PercentageDiscount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PercentageTax).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.HasOne(d => d.Supplier).WithMany(p => p.InventoryIns)
                .HasForeignKey(d => d.SupplierID)
                .HasConstraintName("FK__Inventory__Suppl__0B91BA14");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryIns)
                .HasForeignKey(d => d.WarehouseID)
                .HasConstraintName("FK__Inventory__Wareh__0A9D95DB");
        });

        modelBuilder.Entity<InventoryInDetail>(entity =>
        {
            entity.HasKey(e => e.InventoryInDeID).HasName("PK__Inventor__45E5690D80F381F9");

            entity.ToTable("InventoryInDetail");

            entity.Property(e => e.InventoryInID).HasColumnName("InventoryInID");
            entity.Property(e => e.MaterialID).HasColumnName("MaterialID");
            entity.Property(e => e.UnitID)
                .ValueGeneratedOnAdd()
                .HasColumnName("UnitID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("date");
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.FinalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.InventoryIn).WithMany(p => p.InventoryInDetails)
                .HasForeignKey(d => d.InventoryInID)
                .HasConstraintName("FK__Inventory__Inven__123EB7A3");

            entity.HasOne(d => d.Material).WithMany(p => p.InventoryInDetails)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK__Inventory__Mater__1332DBDC");

            entity.HasOne(d => d.Unit).WithMany(p => p.InventoryInDetails)
                .HasForeignKey(d => d.UnitID)
                .HasConstraintName("FK__Inventory__UnitI__14270015");

        });

        modelBuilder.Entity<InventoryOut>(entity =>
        {
            entity.HasKey(e => e.InventoryOutID).HasName("PK__Inventor__87D08304522CB0F9");

            entity.ToTable("InventoryOut");

            entity.Property(e => e.InventoryOutID).HasColumnName("InventoryOutID");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OutDate).HasColumnType("date");
            entity.Property(e => e.AmountReceived).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FacilityID).HasColumnName("FacilityID");           
            entity.Property(e => e.WarehouseID).HasColumnName("WarehouseID");
            entity.Property(e => e.PercentageDiscount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PercentageTax).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);

            entity.HasOne(d => d.Facility).WithMany(p => p.InventoryOuts)
               .HasForeignKey(d => d.FacilityID)
               .HasConstraintName("FK__Inventory__Facil__0F624AF8");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryOuts)
                .HasForeignKey(d => d.WarehouseID)
                .HasConstraintName("FK__Inventory__Wareh__0E6E26BF");
        });

        modelBuilder.Entity<InventoryOutDetail>(entity =>
        {
            entity.HasKey(e => e.InventoryOutDeID).HasName("PK__Inventor__7FC417D9400FE453");

            entity.ToTable("InventoryOutDetail");

            entity.Property(e => e.InventoryOutDeID).HasColumnName("InventoryOutDeID");
            entity.Property(e => e.InventoryOutID).HasColumnName("InventoryOutID");
            entity.Property(e => e.MaterialID).HasColumnName("MaterialID");
            entity.Property(e => e.UnitID)
                .ValueGeneratedOnAdd()
                .HasColumnName("UnitID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.ModifiedDate).HasColumnType("date");

            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.InventoryOut).WithMany(p => p.InventoryOutDetails)
                .HasForeignKey(d => d.InventoryOutID)

                .HasConstraintName("FK__Inventory__Inven__17036CC0");

            entity.HasOne(d => d.Material).WithMany(p => p.InventoryOutDetails)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK__Inventory__Mater__17F790F9");

            entity.HasOne(d => d.Unit).WithMany(p => p.InventoryOutDetails)
                .HasForeignKey(d => d.UnitID)
                .HasConstraintName("FK__Inventory__UnitI__18EBB532");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialID).HasName("PK__Material__C5061317F95B673A");

            entity.ToTable("Material");

            entity.Property(e => e.MaterialID).HasColumnName("MaterialID");
            entity.Property(e => e.GroupID).HasColumnName("GroupID");
            entity.Property(e => e.MaterialName).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.HasOne(d => d.Group).WithMany(p => p.Materials)
                .HasForeignKey(d => d.GroupID)
                .HasConstraintName("FK__Material__GroupI__01142BA1");
        });

        modelBuilder.Entity<MaterialGroup>(entity =>
        {
            entity.HasKey(e => e.GroupID).HasName("PK__Material__149AF30AC8F9FECF");

            entity.ToTable("MaterialGroup");

            entity.Property(e => e.GroupID).HasColumnName("GroupID");
            entity.Property(e => e.GroupName).HasMaxLength(50);
        });

        modelBuilder.Entity<Organizations>(entity =>
        {
            entity.HasKey(e => e.OrganizationID).HasName("PK__Organiza__CADB0B72F6147E4E");

            entity.Property(e => e.OrganizationID).HasColumnName("OrganizationID");
            entity.Property(e => e.ContactPerson).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(250);
            entity.Property(e => e.OrganizationName).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SchoolID).HasColumnName("SchoolID");

            entity.HasOne(d => d.School).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.SchoolID)
                .HasConstraintName("FK__Organizat__Schoo__6B24EA82");
        });

        modelBuilder.Entity<Photocopier>(entity =>
        {
            entity.HasKey(e => e.PhotocopierID).HasName("PK__Photocop__136EEDC9BCB8E3BD");

            entity.ToTable("Photocopier");

            entity.Property(e => e.PhotocopierID).HasColumnName("PhotocopierID");
            entity.Property(e => e.FacilityID).HasColumnName("FacilityID");
            entity.Property(e => e.PhotocopierName).HasMaxLength(50);
            entity.Property(e => e.SerialNumber).HasMaxLength(20);
            entity.Property(e => e.Location).HasMaxLength(100);

            entity.HasOne(d => d.Facility).WithMany(p => p.Photocopiers)
                .HasForeignKey(d => d.FacilityID)
                .HasConstraintName("FK__Photocopi__Facil__70DDC3D8");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptID).HasName("PK__Receipt__CC08C4001696DF3D");

            entity.ToTable("Receipt");

            entity.Property(e => e.ReceiptID).HasColumnName("ReceiptID");
            entity.Property(e => e.DepositPayment).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PercentageDiscount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PercentageTax).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TaxAmount)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.ReceiptDate).HasColumnType("date");

            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserID).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.UserID)
                .HasConstraintName("FK__Receipt__UserID__6E01572D");
        });

        modelBuilder.Entity<ReceiptDetail>(entity =>
        {
            entity.HasKey(e => e.ReceiptDeID).HasName("PK__ReceiptD__B84A11E3EF0FDA16");

            entity.ToTable("ReceiptDetail");

            entity.Property(e => e.ReceiptDeID).HasColumnName("ReceiptDeID");
            entity.Property(e => e.ReceiptID).HasColumnName("ReceiptID");
            entity.Property(e => e.ServiceID).HasColumnName("ServiceID");
            entity.Property(e => e.FinalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PhotocopierID)
                .ValueGeneratedOnAdd()
                .HasColumnName("PhotocopierID");

            entity.HasOne(d => d.Photocopier).WithMany(p => p.ReceiptDetails)
                .HasForeignKey(d => d.PhotocopierID)
                .HasConstraintName("FK__ReceiptDe__Photo__75A278F5");

            entity.HasOne(d => d.Receipt).WithMany(p => p.ReceiptDetails)
                .HasForeignKey(d => d.ReceiptID)
                .HasConstraintName("FK__ReceiptDe__Recei__73BA3083");

            entity.HasOne(d => d.Services).WithMany(p => p.ReceiptDetails)
                .HasForeignKey(d => d.ServiceID)
                .HasConstraintName("FK__ReceiptDe__Servi__74AE54BC");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleID).HasName("PK__Role__8AFACE3AEED1E608");

            entity.ToTable("Role");

            entity.Property(e => e.RoleID).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.SchoolID).HasName("PK__School__3DA4677BE3E09D92");

            entity.ToTable("School");

            entity.Property(e => e.SchoolID).HasColumnName("SchoolID");
            entity.Property(e => e.SchoolName).HasMaxLength(250);
        });

        modelBuilder.Entity<Services>(entity =>
        {
            entity.HasKey(e => e.ServiceID).HasName("PK__Service__C51BB0EA963BCD44");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceID).HasColumnName("ServiceID");
            entity.Property(e => e.GroupID).HasColumnName("GroupID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Dvt).HasMaxLength(50);
            entity.Property(e => e.ServiceName).HasMaxLength(100);

            entity.Property(e => e.FacilityID).HasColumnName("FacilityID");
            entity.HasOne(d => d.Group).WithMany(p => p.Services)
                .HasForeignKey(d => d.GroupID)
                .HasConstraintName("FK__Service__GroupID__66603565");
            entity.HasOne(d => d.Facility).WithMany(p => p.Services)
                .HasForeignKey(d => d.FacilityID)
                .HasConstraintName("FK__Photocopi__Facil");
        });

        modelBuilder.Entity<ServiceGroup>(entity =>
        {
            entity.HasKey(e => e.GroupID).HasName("PK__ServiceG__149AF30AAAEF8A98");

            entity.ToTable("ServiceGroup");

            entity.Property(e => e.GroupID).HasColumnName("GroupID");
            entity.Property(e => e.GroupName).HasMaxLength(100);
        });

        modelBuilder.Entity<Suppliers>(entity =>
        {
            entity.HasKey(e => e.SupplierID).HasName("PK__Supplier__4BE66694A1170512");

            entity.Property(e => e.SupplierID).HasColumnName("SupplierID");
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.ContactName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.SupplierName).HasMaxLength(100);
        });

        modelBuilder.Entity<UnitOfMeasure>(entity =>
        {
            entity.HasKey(e => e.UnitID).HasName("PK__UnitOfMe__44F5EC95B507D6B1");

            entity.ToTable("UnitOfMeasure");

            entity.Property(e => e.UnitID).HasColumnName("UnitID");
            entity.Property(e => e.ConversionFactor).HasMaxLength(50);
            entity.Property(e => e.UnitName).HasMaxLength(50);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PK__Users__1788CCAC2585431B");

            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CodeUser)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.OrganizationID).HasColumnName("OrganizationID");
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Organization).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganizationID)
                .HasConstraintName("FK_Users_Organizations");
        });

        modelBuilder.Entity<UserFacilities>(entity =>
        {
            entity.HasKey(e => e.UserFaID).HasName("PK__UserFaci__4273C4154550321C");

            entity.Property(e => e.UserFaID).HasColumnName("UserFaID");
            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.FacilityID)
                .ValueGeneratedOnAdd()
                .HasColumnName("FacilityID");

            entity.HasOne(d => d.Facility).WithMany(p => p.UserFacilities)
                .HasForeignKey(d => d.FacilityID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFacil__Facil__5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.UserFacilities)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFacil__UserI__534D60F1");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleID).HasName("PK__UserRoleID__2608AFD929030CC6");

            entity.ToTable("UserRole");

            entity.Property(e => e.UserRoleID).HasColumnName("UserRoleID");
            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.RoleID)
                .ValueGeneratedOnAdd()
                .HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRole__RoleID__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRole__UserID__4D94879B");
        });

        modelBuilder.Entity<Warehouses>(entity =>
        {
            entity.HasKey(e => e.WarehouseID).HasName("PK__Warehous__2608AFD929030CC6");

            entity.Property(e => e.WarehouseID).HasColumnName("WarehouseID");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.ManagerNameWh).HasMaxLength(100);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.WarehouseName).HasMaxLength(50);

           
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
