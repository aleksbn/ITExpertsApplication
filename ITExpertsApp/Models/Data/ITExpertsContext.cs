namespace ITExpertsApp.Models.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ITExpertsContext : DbContext
    {
        public ITExpertsContext()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<DevPath> DevPaths { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Technology> Technologies { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WorkingAt> WorkingAts { get; set; }
        public virtual DbSet<UserOnPath> UserOnPaths { get; set; }
        public virtual DbSet<Title> Titles { get; set; }
        public virtual DbSet<UserSkill> UserSkills { get; set; }
        public virtual DbSet<JobRequest> JobRequests { get; set; }

        //public System.Data.Entity.DbSet<ITExpertsApp.Models.ViewModels.Account.EditUserVM> EditUserVMs { get; set; }

        //public System.Data.Entity.DbSet<ITExpertsApp.Models.ViewModels.Account.UserVM> UserVMs { get; set; }

        //public System.Data.Entity.DbSet<ITExpertsApp.Models.ViewModels.CompanyVM> CompanyVMs { get; set; }

        //public System.Data.Entity.DbSet<ITExpertsApp.Models.ViewModels.Account.EditCompanyVM> EditCompanyVMs { get; set; }

        //public System.Data.Entity.DbSet<ITExpertsApp.Models.ViewModels.Account.JobRequestVM> JobRequestVMs { get; set; }
    }
}
