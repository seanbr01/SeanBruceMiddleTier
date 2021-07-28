using Microsoft.EntityFrameworkCore;
using SeanBruceMiddleTier.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeanBruceMiddleTier.Data
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructor
        public ApplicationDbContext() : base() { }
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        #endregion Constructor

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Map Entity names to DB Table names
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<People>().ToTable("Peoples");
        }
        #endregion Methods

        #region Properties
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<People> Peoples { get; set; }
        #endregion Properties
    }
}
