using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureADTenantPart2.Models.ConData
{
    [Table("Offices", Schema = "dbo")]
    public partial class Office
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OfficeID { get; set; }

        [Required]
        public string OfficeName { get; set; }

        [Required]
        public string OfficeAddress { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string TelephoneNumber { get; set; }

    }
}