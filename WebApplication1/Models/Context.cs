using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CPEApi.Models
{
    public partial class Context : DbContext
    {
        public Context()
        {
        }

        public Context(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Anfragen> Anfragens { get; set; } = null!;
        public virtual DbSet<Antworten> Antwortens { get; set; } = null!;
        public virtual DbSet<Antworten2> Antworten2s { get; set; } = null!;
        public virtual DbSet<Cpe> Cpes { get; set; } = null!;
        public virtual DbSet<Tfproduct> Tfproducts { get; set; } = null!;
        public virtual DbSet<Tftitle> Tftitles { get; set; } = null!;
        public virtual DbSet<Tfvendor> Tfvendors { get; set; } = null!;
        public virtual DbSet<Typ> Typs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:Context");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Anfragen>(entity =>
            {
                entity.ToTable("Anfragen");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Part).HasMaxLength(100);

                entity.Property(e => e.Product).HasMaxLength(100);

                entity.Property(e => e.Vendor).HasMaxLength(100);

                entity.Property(e => e.Version).HasMaxLength(100);
            });

            modelBuilder.Entity<Antworten>(entity =>
            {
                entity.ToTable("Antworten");

                entity.Property(e => e.Cpeid).HasColumnName("CPEId");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.Anfrage)
                    .WithMany(p => p.Antwortens)
                    .HasForeignKey(d => d.AnfrageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Antworten_Anfrage");

                entity.HasOne(d => d.Cpe)
                    .WithMany(p => p.Antwortens)
                    .HasForeignKey(d => d.Cpeid)
                    .HasConstraintName("FK_CPE_Anfrage");

                entity.HasOne(d => d.TypNavigation)
                    .WithMany(p => p.Antwortens)
                    .HasForeignKey(d => d.Typ)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Antwort_Typ");
            });

            modelBuilder.Entity<Antworten2>(entity =>
            {
                entity.ToTable("Antworten2");

                entity.Property(e => e.Cpeid).HasColumnName("CPEId");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.HasOne(d => d.Anfrage)
                    .WithMany(p => p.Antworten2s)
                    .HasForeignKey(d => d.AnfrageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Antworten2_Anfrage");

                entity.HasOne(d => d.Cpe)
                    .WithMany(p => p.Antworten2s)
                    .HasForeignKey(d => d.Cpeid)
                    .HasConstraintName("FK_Antworten2_CPE");

                entity.HasOne(d => d.TypNavigation)
                    .WithMany(p => p.Antworten2s)
                    .HasForeignKey(d => d.Typ)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Antworten2_Typ");
            });

            modelBuilder.Entity<Cpe>(entity =>
            {
                entity.ToTable("CPEs");

                entity.HasIndex(e => e.Product, "indexprod");

                entity.Property(e => e.Edition).HasMaxLength(100);

                entity.Property(e => e.Language).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(1000);

                entity.Property(e => e.Other).HasMaxLength(100);

                entity.Property(e => e.Part).HasMaxLength(100);

                entity.Property(e => e.Product).HasMaxLength(100);

                entity.Property(e => e.SwEdition)
                    .HasMaxLength(100)
                    .HasColumnName("Sw_edition");

                entity.Property(e => e.TargetHw)
                    .HasMaxLength(100)
                    .HasColumnName("Target_hw");

                entity.Property(e => e.TargetSw)
                    .HasMaxLength(100)
                    .HasColumnName("Target_sw");

                entity.Property(e => e.Title).HasMaxLength(1000);

                entity.Property(e => e.Update).HasMaxLength(100);

                entity.Property(e => e.Vendor).HasMaxLength(100);

                entity.Property(e => e.Version).HasMaxLength(100);
            });

            modelBuilder.Entity<Tfproduct>(entity =>
            {
                entity.ToTable("TFProduct");

                entity.Property(e => e.DoubleNormalized).HasColumnName("doubleNormalized");

                entity.Property(e => e.LogNormalized).HasColumnName("logNormalized");

                entity.Property(e => e.RawCount).HasColumnName("rawCount");

                entity.Property(e => e.Term).HasMaxLength(100);

                entity.Property(e => e.TfIdf).HasColumnName("tfIdf");

                entity.Property(e => e.TfIdfCount).HasColumnName("tfIdfCount");

                entity.Property(e => e.TfIdfLogNorm).HasColumnName("tfIdfLogNorm");

                entity.Property(e => e.TfIdfdoubleNorm).HasColumnName("tfIdfdoubleNorm");
            });

            modelBuilder.Entity<Tftitle>(entity =>
            {
                entity.ToTable("TFTitle");

                entity.Property(e => e.DoubleNormalized).HasColumnName("doubleNormalized");

                entity.Property(e => e.LogNormalized).HasColumnName("logNormalized");

                entity.Property(e => e.RawCount).HasColumnName("rawCount");

                entity.Property(e => e.Term).HasMaxLength(100);

                entity.Property(e => e.TfIdf).HasColumnName("tfIdf");

                entity.Property(e => e.TfIdfCount).HasColumnName("tfIdfCount");

                entity.Property(e => e.TfIdfLogNorm).HasColumnName("tfIdfLogNorm");

                entity.Property(e => e.TfIdfdoubleNorm).HasColumnName("tfIdfdoubleNorm");
            });

            modelBuilder.Entity<Tfvendor>(entity =>
            {
                entity.ToTable("TFVendor");

                entity.Property(e => e.DoubleNormalized).HasColumnName("doubleNormalized");

                entity.Property(e => e.LogNormalized).HasColumnName("logNormalized");

                entity.Property(e => e.RawCount).HasColumnName("rawCount");

                entity.Property(e => e.Term).HasMaxLength(100);

                entity.Property(e => e.TfIdf).HasColumnName("tfIdf");

                entity.Property(e => e.TfIdfCount).HasColumnName("tfIdfCount");

                entity.Property(e => e.TfIdfLogNorm).HasColumnName("tfIdfLogNorm");

                entity.Property(e => e.TfIdfdoubleNorm).HasColumnName("tfIdfdoubleNorm");
            });

            modelBuilder.Entity<Typ>(entity =>
            {
                entity.ToTable("Typ");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
