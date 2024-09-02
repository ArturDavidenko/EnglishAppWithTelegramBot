using EnglishTelegramBotAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EnglishTelegramBotAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Word> words { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Word>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<Word>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
