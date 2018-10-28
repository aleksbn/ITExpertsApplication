using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.Data
{
    [Table("JobRequest")]
    public partial class JobRequest
    {
        [Key, Column(Order = 0)]
        public int SenderId { get; set; }

        [Key, Column(Order = 1)]
        public int ReceiverId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date of request")]
        public DateTime DateOfRequest { get; set; }

        public string Status { get; set; }

        public string Techs { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }
    }
}