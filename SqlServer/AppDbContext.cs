using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace SqlServer
{
    public class AppDbContext : DbContext
    {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }

            public DbSet<Mood> Moods { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Mood>(entity =>
                {
                    entity.HasKey(e => e.ID);

                    entity.Property(e => e.ID)
                        .ValueGeneratedOnAdd();

                    entity.Property(e => e.Date)
                        .IsRequired();

                    entity.Property(e => e.MoodType)
                        .IsRequired()
                        .HasConversion<int>();

                    entity.Property(e => e.MoodQuantity)
                        .IsRequired();
                });
            }
        }
    }
