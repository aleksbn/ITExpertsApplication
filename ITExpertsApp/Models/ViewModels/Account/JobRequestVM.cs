using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels.Account
{
    public class JobRequestVM
    {
        [Key, Column(Order = 0)]
        public int SenderId { get; set; }

        [Key, Column(Order = 1)]
        public int ReceiverId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date of request")]
        public DateTime DateOfRequest { get; set; }

        [StringLength(10, ErrorMessage = "You have to use up to {0} letters!")]
        public string Status { get; set; }

        public string Techs { get; set; }
    }
}