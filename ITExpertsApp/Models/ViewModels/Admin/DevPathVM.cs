using ITExpertsApp.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels
{
    public class DevPathVM
    {
        public DevPathVM()
        {

        }

        public DevPathVM(DevPath row)
        {
            PathId = row.PathId;
            PathName = row.PathName;
        }

        public int PathId { get; set; }

        [Required]
        [StringLength(50)]
        public string PathName { get; set; }
    }
}