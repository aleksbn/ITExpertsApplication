using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.Data
{
    [Table("UserHasSkill")]
    public class UserSkill
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        public int TechId { get; set; }

        [Key, Column(Order = 2)]
        public int JobId { get; set; }
    }
}