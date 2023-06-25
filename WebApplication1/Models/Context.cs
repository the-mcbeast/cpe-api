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

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Cpe> Cpes { get; set; } = null!;
        public virtual DbSet<Tfproduct> Tfproducts { get; set; } = null!;
        public virtual DbSet<Tftitle> Tftitles { get; set; } = null!;
        public virtual DbSet<Tfvendor> Tfvendors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:Context");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

                entity.HasIndex(e => e.Term, "indexterm");

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
