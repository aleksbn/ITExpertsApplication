using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels;

namespace ITExpertsApp.Models.ViewModels
{
    public class TechnologyVM
    {
        [Key]
        public int TechId { get; set; }

        [Required]
        [StringLength(50)]
        public string TechName { get; set; }

        [Required]
        [StringLength(300)]
        public string TechDescription { get; set; }

        [Required]
        public int PathId { get; set; }

        public TechnologyVM()
        {

        }

        public TechnologyVM(Technology row)
        {
            TechId = row.TechId;
            TechName = row.TechName;
            TechDescription = row.TechDescription;
            PathId = row.DevPath.PathId;
        }
    }
}