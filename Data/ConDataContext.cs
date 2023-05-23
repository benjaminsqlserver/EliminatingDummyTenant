using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using AzureADTenantPart2.Models.ConData;

namespace AzureADTenantPart2.Data
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
            this.OnModelBuilding(builder);
        }

        public DbSet<AzureADTenantPart2.Models.ConData.AllowedTenant> AllowedTenants { get; set; }

        public DbSet<AzureADTenantPart2.Models.ConData.Office> Offices { get; set; }

        public DbSet<AzureADTenantPart2.Models.ConData.SolutionUser> SolutionUsers { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}