using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels.Search
{
    public class TestClass
    {
        [Key]
        [Required]
        public int Broj { get; set; }
        [Required]
        public bool TrueFalse { get; set; }
        [Required]
        public string Tekst { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public double Novac { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}