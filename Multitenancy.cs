using System.Collections.ObjectModel;

namespace AzureADTenantPart2
{
    public class Tenant
    {
        public string Name { get; set; }
        public string[] Hostnames { get; set; }
        public string ConnectionString { get; set; }
    }

    public class Multitenancy
    {
        public Collection<Tenant> Tenants { get; set; }
    }
}
