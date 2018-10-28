using ITExpertsApp.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels
{
    public class CompanyVM
    {
        public CompanyVM()
        {

        }

        public CompanyVM(Company row)
        {
            CompanyId = row.CompanyId;
            CompanyName = row.CompanyName;
            Email = row.Email;
            Budget = row.Budget;
            Description = row.Description;
            MadeUp = row.MadeUp;
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

        [Display(Name = "Company salary monthly budget")]
        [Required(ErrorMessage = "You need to input the budget of your company!")]
        public decimal Budget { get; set; }

        [Required(ErrorMessage = "You need to input the description of your company!")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Description must be up to 500 letters.")]
        public string Description { get; set; }

        public double CurrentSalaries { get; set; }

        public int Employees { get; set; }

        public bool MadeUp { get; set; }
    }
}