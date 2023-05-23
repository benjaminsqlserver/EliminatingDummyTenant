using Microsoft.EntityFrameworkCore;

namespace AzureADTenantPart2.Data
{
    public partial class ConDataContext
    {
        private readonly string _connectionString;

        public ConDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
