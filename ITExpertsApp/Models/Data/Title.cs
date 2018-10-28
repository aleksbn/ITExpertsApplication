using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.Data
{
    public class Title
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The education title must be entered")]
        [StringLength(50, ErrorMessage = "The lenght of the title must be between {0} and {2}", MinimumLength = 5)]
        public string EducationTitle { get; set; }
    }
}