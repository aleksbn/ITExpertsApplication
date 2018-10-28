namespace ITExpertsApp.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Technology")]
    public partial class Technology
    {
        [Key]
        public int TechId { get; set; }

        [Required]
        [StringLength(50)]
        public string TechName { get; set; }

        [Required]
        [StringLength(300)]
        public string TechDescription { get; set; }

        public int PathId { get; set; }

        [ForeignKey("PathId")]
        public virtual DevPath DevPath { get; set; }
    }
}
