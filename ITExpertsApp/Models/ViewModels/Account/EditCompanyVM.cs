using ITExpertsApp.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels.Account
{
    public class EditCompanyVM
    {
        public EditCompanyVM()
        {

        }

        public EditCompanyVM(Company row)
        {
            CompanyId = row.CompanyId;
            CompanyName = row.CompanyName;
            Email = row.Email;
            Description = row.Description;
            OldPassword = "";
            Password = "";
            ConfirmPassword = "";
            Active = row.Active;
            Budget = row.Budget;
            IsCompleted = row.IsCompleted;
        }

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

        [Display(Name = "Old password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Company salary monthly budget")]
        public decimal Budget { get; set; }

        public bool? Active { get; set; }

        public bool Blocked { get; set; }

        [StringLength(500, ErrorMessage = "Description must be up to 500 letters.")]
        public string Description { get; set; }

        public bool? IsCompleted { get; set; }
    }
}