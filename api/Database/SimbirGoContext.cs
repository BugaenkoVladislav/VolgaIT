using System;
using System.Collections.Generic;
using api.Database.Models;
using api.Database.Views;
using Microsoft.EntityFrameworkCore;

namespace api.Database;

public partial class SimbirGoContext : DbContext
{
    public SimbirGoContext()
    {
    }

    public SimbirGoContext(DbContextOptions<SimbirGoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Rent> Rents { get; set; }

    public virtual DbSet<RentInfo> RentInfos { get; set; }

    public virtual DbSet<RentType> RentTypes { get; set; }

    public virtual DbSet<Transport> Transports { get; set; }

    public virtual DbSet<TransportInfo> TransportInfos { get; set; }

    public virtual DbSet<TransportType> TransportTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Database=Simbir.GO;Password=Chmonya");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Colors_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Color1).HasColumnName("color");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Models_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Model1).HasColumnName("model");
        });

        modelBuilder.Entity<Rent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Rents_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.FinalPrice).HasColumnName("finalPrice");
            entity.Property(e => e.IdTransport).HasColumnName("idTransport");
            entity.Property(e => e.IdUser).HasColumnName("idUser");
            entity.Property(e => e.PriceOfUnit).HasColumnName("priceOfUnit");
            entity.Property(e => e.PriceType).HasColumnName("priceType");
            entity.Property(e => e.TimeEnd).HasColumnName("timeEnd");
            entity.Property(e => e.TimeStart).HasColumnName("timeStart");

            entity.HasOne(d => d.IdTransportNavigation).WithMany(p => p.Rents)
                .HasForeignKey(d => d.IdTransport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Rents_idTransport_fkey");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Rents)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Rents_idUser_fkey");

            entity.HasOne(d => d.PriceTypeNavigation).WithMany(p => p.Rents)
                .HasForeignKey(d => d.PriceType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Rents_priceType_fkey");
        });

        modelBuilder.Entity<RentInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RentInfo");

            entity.Property(e => e.FinalPrice).HasColumnName("finalPrice");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdTransport).HasColumnName("idTransport");
            entity.Property(e => e.Owner).HasColumnName("owner");
            entity.Property(e => e.PriceOfUnit).HasColumnName("priceOfUnit");
            entity.Property(e => e.PriceType).HasColumnName("priceType");
            entity.Property(e => e.TimeEnd).HasColumnName("timeEnd");
            entity.Property(e => e.TimeStart).HasColumnName("timeStart");
            entity.Property(e => e.User).HasColumnName("user");
        });

        modelBuilder.Entity<RentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RentTypes_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.RentType1).HasColumnName("rentType");
        });

        modelBuilder.Entity<Transport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Transports_pkey");

            entity.HasIndex(e => e.Identifier, "Transports_identifier_identifier1_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CanBeRented).HasColumnName("canBeRented");
            entity.Property(e => e.DayPrice).HasColumnName("dayPrice");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IdColor).HasColumnName("idColor");
            entity.Property(e => e.IdModel).HasColumnName("idModel");
            entity.Property(e => e.IdOwner).HasColumnName("idOwner");
            entity.Property(e => e.IdTransportType).HasColumnName("idTransportType");
            entity.Property(e => e.Identifier).HasColumnName("identifier");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.MinutePrice).HasColumnName("minutePrice");

            entity.HasOne(d => d.IdColorNavigation).WithMany(p => p.Transports)
                .HasForeignKey(d => d.IdColor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transports_idColor_fkey");

            entity.HasOne(d => d.IdModelNavigation).WithMany(p => p.Transports)
                .HasForeignKey(d => d.IdModel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transports_idModel_fkey");

            entity.HasOne(d => d.IdOwnerNavigation).WithMany(p => p.Transports)
                .HasForeignKey(d => d.IdOwner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transports_idOwner_fkey");

            entity.HasOne(d => d.IdTransportTypeNavigation).WithMany(p => p.Transports)
                .HasForeignKey(d => d.IdTransportType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transports_idTransportType_fkey");
        });

        modelBuilder.Entity<TransportInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TransportInfo");

            entity.Property(e => e.CanBeRented).HasColumnName("canBeRented");
            entity.Property(e => e.Color).HasColumnName("color");
            entity.Property(e => e.DayPrice).HasColumnName("dayPrice");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Identifier).HasColumnName("identifier");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.MinutePrice).HasColumnName("minutePrice");
            entity.Property(e => e.Model).HasColumnName("model");
            entity.Property(e => e.TransportType).HasColumnName("transportType");
        });

        modelBuilder.Entity<TransportType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TransportTypes_pkey");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.TransportType1).HasColumnName("transportType");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.HasIndex(e => e.Username, "Users_username_username1_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Balance).HasColumnName("balance");
            entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
