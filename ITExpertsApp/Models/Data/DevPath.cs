namespace ITExpertsApp.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DevPath")]
    public partial class DevPath
    {
        [Key]
        public int PathId { get; set; }

        [Required]
        [StringLength(50)]
        public string PathName { get; set; }
    }
}
