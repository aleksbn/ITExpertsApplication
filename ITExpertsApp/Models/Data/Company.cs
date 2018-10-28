namespace ITExpertsApp.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Company")]
    public partial class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Display(Name = "Company name")]
        [Required(ErrorMessage = "You need to input the name of your company!")]
        [StringLength(50, ErrorMessage = "You have to use from {2} to {0} letters!", MinimumLength = 5)]
        public string CompanyName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "You need to input the email of your company!")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessage = "You have to use from {2} to {0} letters!", MinimumLength = 5)]
        public string Email { get; set; }

        [Required(ErrorMessage = "You need to input the password of your company!")]
        [StringLength(50, ErrorMessage = "You have to use from {2} to {0} letters!", MinimumLength = 5)]
        public string Password { get; set; }

        [Display(Name = "Company salary monthly budget")]
        public decimal Budget { get; set; }

        public bool? Active { get; set; }

        public bool Blocked { get; set; }

        [StringLength(500, ErrorMessage = "Description must be up to 500 letters.")]
        public string Description { get; set; }

        public bool? IsCompleted { get; set; }

        public bool MadeUp { get; set; }
    }
}
