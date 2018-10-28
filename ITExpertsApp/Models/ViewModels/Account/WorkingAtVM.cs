using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels.Account
{
    public class WorkingAtVM
    {
        public int JobId { get; set; }

        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public int PathId { get; set; }

        public int[] TechId { get; set; }

        [Display(Name = "Company name")]
        [StringLength(50, ErrorMessage = "You have to use from {2} to {0} letters!", MinimumLength = 5)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "You need to input the date since you worked there.")]
        [DataType(DataType.Date)]
        public DateTime Since { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Until { get; set; }

        [Display(Name = "Job description")]
        [StringLength(200, ErrorMessage = "You have to use from {2} to {0} letters!", MinimumLength = 5)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string[] Technology { get; set; }

        public WorkingAtVM()
        {
            
        }

        public WorkingAtVM(WorkingAt job)
        {
            ITExpertsContext db = new ITExpertsContext();
            JobId = job.JobId;
            CompanyId = job.CompanyId;
            CompanyName = job.CompanyName;
            Description = job.Description;
            PathId = job.Tech.PathId;
            Since = job.Since;
            Until = job.Until;
            UserId = job.UserId;
            WorkingAt[] listToConvert = db.WorkingAts.ToList().Where(x => x.UserId == UserId && x.CompanyName == CompanyName && x.Since == Since).OrderBy(x => x.TechId).ToArray();
            TechId = new int[listToConvert.Count()];
            for (int i = 0; i < listToConvert.Count(); i++)
            {
                TechId[i] = listToConvert[i].TechId;
            }
            db.Dispose();
        }
    }
}