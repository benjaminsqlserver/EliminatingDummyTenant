using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureADTenantPart2.Models.ConData
{
    [Table("SolutionUsers", Schema = "dbo")]
    public partial class SolutionUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserID { get; set; }

        [Required]
        public string EmailAddress { get; set; }

    }
}