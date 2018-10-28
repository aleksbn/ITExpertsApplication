using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels.Search
{
    public class SearchUser
    {
        public bool UnemployedOnly { get; set; }

        public bool Newcomers { get; set; }

        public double MinSalary { get; set; }

        public double MaxSalary { get; set; }

        public int DevPath { get; set; }

        public int[] Technologies { get; set; }

        public int YearsOfWork { get; set; }

        public int EducationLevel { get; set; }
    }
}