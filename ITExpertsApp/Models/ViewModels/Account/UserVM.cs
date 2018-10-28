using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITExpertsApp.Models.ViewModels.Account
{
    public class UserVM
    {
        public UserVM()
        {

        }

        public UserVM(User row)
        {
            UserId = row.UserId;
            RoleId = row.RoleId;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Email = row.Email;
            Description = row.Description;
            Password = row.Password;
            Active = row.Active;
            LinkedIn = row.LinkedIn;
            StackOverflow = row.StackOverflow;
            DateOfBirth = row.DateOfBirth;
            EducationLevel = row.EducationLevel;
            Title = row.Title;
            AverageGrade = row.AverageGrade;
            IsCompleted = row.IsCompleted;
            Salary = HelpClass.ReturnSalary(UserId);
            Jobs = HelpClass.ReturnJobs(UserId);
            Blocked = row.Blocked;
            IsRegistered = row.IsRegistered;
            MadeUp = row.MadeUp;
        }

        [Key]
        public int UserId { get; set; }

        public int RoleId { get; set; }

        [Required(ErrorMessage = "You must enter your first name!")]
        [Display(Name = "First name")]
        [StringLength(50, ErrorMessage = "First name must be from {2} to {0} letters.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You must enter your last name!")]
        [Display(Name = "Last name")]
        [StringLength(50, ErrorMessage = "Last name must be from {2} to {0} letters.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must enter your email address!")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, ErrorMessage = "Email must be from {2} to {0} letters.", MinimumLength = 5)]
        public string Email { get; set; }

        [StringLength(300, ErrorMessage = "Description must be up to {0} letters.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "You must enter your password!")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Password must be from {2} to {0} letters.", MinimumLength = 6)]
        public string Password { get; set; }

        public bool? Active { get; set; }

        [StringLength(100, ErrorMessage = "LinkedIn link must be up to {0} letters.")]
        [Display(Name = "LinkedIn profile")]
        public string LinkedIn { get; set; }

        [StringLength(100, ErrorMessage = "StackOverflow link must be up to {0} letters.")]
        [Display(Name = "Stackoverflow profile")]
        public string StackOverflow { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "You must enter your degree level!")]
        [Display(Name = "Degree level")]
        public int EducationLevel { get; set; }

        [Required(ErrorMessage = "You must enter your title!")]
        [Display(Name = "Title obtained")]
        [StringLength(50, ErrorMessage = "Title must be from {2} to {0} letters.", MinimumLength = 5)]
        public string Title { get; set; }

        [Display(Name = "Average grade")]
        [Required(ErrorMessage = "You must enter your average grade!")]
        public double AverageGrade { get; set; }

        public double Salary { get; set; }

        public bool? IsCompleted { get; set; }

        public bool Blocked { get; set; }

        public bool IsRegistered { get; set; }

        public bool MadeUp { get; set; }

        public WorkingAtVM[] Jobs { get; set; }
    }
}