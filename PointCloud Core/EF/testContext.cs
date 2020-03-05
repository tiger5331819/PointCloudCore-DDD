using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PointCloudCore.EF
{
    public partial class testContext : DbContext
    {
        public testContext()
        {
        }

        public testContext(DbContextOptions<testContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChallengerList> ChallengerList { get; set; }
        public virtual DbSet<Rounds> Rounds { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<StudentRoundsview> StudentRoundsview { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=MSI-SU;Database=test;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChallengerList>(entity =>
            {
                entity.ToTable("challengerList");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Challenger1)
                    .HasColumnName("challenger1")
                    .HasMaxLength(50);

                entity.Property(e => e.Challenger2)
                    .HasColumnName("challenger2")
                    .HasMaxLength(50);

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.Rounds).HasColumnName("rounds");
            });

            modelBuilder.Entity<Rounds>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Challenger1)
                    .HasColumnName("challenger1")
                    .HasMaxLength(50);

                entity.Property(e => e.Challenger2)
                    .HasColumnName("challenger2")
                    .HasMaxLength(50);

                entity.Property(e => e.Result)
                    .HasColumnName("result")
                    .HasMaxLength(50);

                entity.Property(e => e.Rounds1).HasColumnName("rounds");

                entity.Property(e => e.Winner)
                    .HasColumnName("winner")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("student");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Record).HasColumnName("record");

                entity.Property(e => e.Sex)
                    .HasColumnName("sex")
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<StudentRoundsview>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("studentRoundsview");

                entity.Property(e => e.Challenger2)
                    .HasColumnName("challenger2")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Result)
                    .HasColumnName("result")
                    .HasMaxLength(50);

                entity.Property(e => e.Rounds).HasColumnName("rounds");

                entity.Property(e => e.Winner)
                    .HasColumnName("winner")
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
