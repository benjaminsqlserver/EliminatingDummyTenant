using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureADTenantPart2.Models.ConData
{
    [Table("AllowedTenants", Schema = "dbo")]
    public partial class AllowedTenant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TenantID { get; set; }

        [Required]
        public string DomainName { get; set; }

    }
}