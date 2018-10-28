using ITExpertsApp.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels
{
    public class ModeratorVM
    {
        public ModeratorVM()
        {

        }

        public ModeratorVM(User row)
        {
            UserId = row.UserId;
            RoleId = row.RoleId;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Email = row.Email;
            Password = row.Password;
            Active = row.Active;
            Blocked = row.Blocked;
        }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        [Required(ErrorMessage = "You must enter your first name!")]
        [StringLength(50, ErrorMessage = "First name must be from {2} to {0} letters.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You must enter your last name!")]
        [StringLength(50, ErrorMessage = "Last name must be from {2} to {0} letters.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must enter your email address!")]
        [StringLength(50, ErrorMessage = "Email must be from {2} to {0} letters.", MinimumLength = 5)]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must enter your password!")]
        [StringLength(50, ErrorMessage = "Password must be from {2} to {0} letters.", MinimumLength = 2)]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must enter the confirmation of password!")]
        [StringLength(50, ErrorMessage = "Password must be from {2} to {0} letters.", MinimumLength = 2)]
        public string ConfirmPassword { get; set; }

        public bool? Active { get; set; }

        public bool Blocked { get; set; }

        public string Reason { get; set; }
    }
}