namespace ITExpertsApp.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkingAt")]
    public partial class WorkingAt
    {
        [Key]
        public int JobId { get; set; }

        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public int TechId { get; set; }

        [Display(Name = "Company name")]
        [Required(ErrorMessage = "You need to input the name of your company!")]
        [StringLength(50, ErrorMessage = "You have to use from {2} to {0} letters!", MinimumLength = 5)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "You need to input the date since you worked there.")]
        [DataType(DataType.Date)]
        public DateTime Since { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Until { get; set; }

        [Display(Name = "Job description")]
        [StringLength(200, ErrorMessage = "You have to use from {2} to {0} letters!", MinimumLength = 0)]
        public string Description { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [ForeignKey("TechId")]
        public virtual Technology Tech { get; set; }
    }
}
