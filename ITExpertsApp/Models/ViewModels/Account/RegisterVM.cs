using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels
{
    public class RegisterVM
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Company name")]
        public string CompanyName { get; set; }

        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessage = "Email must be from {2} to {0} letters.", MinimumLength = 5)]
        public string UserEmail { get; set; }

        [Display(Name = "Email confirmation")]
        [Compare("UserEmail", ErrorMessage = "Email and its confirmation must be the same!")]
        public string UserEmailConfirm { get; set; }

        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessage = "Email must be from {2} to {0} letters.", MinimumLength = 5)]
        public string CompanyEmail { get; set; }

        [Display(Name = "Email confirmation")]
        [Compare("CompanyEmail", ErrorMessage = "Email and its confirmation must be the same!")]
        public string CompanyEmailConfirm { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Password must be from {2} to {0} letters.", MinimumLength = 2)]
        public string UserPassword { get; set; }

        [Display(Name = "Password confirmation")]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Password and its confirmation must match!")]
        public string UserPasswordConfirmed { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Password must be from {2} to {0} letters.", MinimumLength = 2)]
        public string CompanyPassword { get; set; }

        [Display(Name = "Password confirmation")]
        [DataType(DataType.Password)]
        [Compare("CompanyPassword", ErrorMessage = "Password and its confirmation must match!")]
        public string CompanyPasswordConfirmed { get; set; }

        [Display(Name = "Company salary monthly budget")]
        public decimal Budget { get; set; }
    }
}