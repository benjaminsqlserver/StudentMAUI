using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using StudentMAUI.Models.ConData;

namespace StudentMAUI.Data
{
    public partial class ConDataContext : DbContext
    {
        public ConDataContext()
        {
        }

        public ConDataContext(DbContextOptions<ConDataContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StudentMAUI.Models.ConData.Student>()
              .HasOne(i => i.Gender)
              .WithMany(i => i.Students)
              .HasForeignKey(i => i.GenderID)
              .HasPrincipalKey(i => i.GenderID);
            this.OnModelBuilding(builder);
        }

        public DbSet<StudentMAUI.Models.ConData.Gender> Genders { get; set; }

        public DbSet<StudentMAUI.Models.ConData.Student> Students { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}