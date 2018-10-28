using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITExpertsApp.Models.ViewModels.Search
{
    public static class HelpClass
    {
        public static double ReturnSalary(int id)
        {
            double salary = 500;
            try
            {
                using (ITExpertsContext db = new ITExpertsContext())
                {
                    User user = db.Users.Find(id);
                    int coefficient;
                    switch (user.EducationLevel)
                    {
                        case 2: coefficient = 25; break;
                        case 3: coefficient = 30; break;
                        case 4: coefficient = 35; break;
                        case 5: coefficient = 40; break;
                        case 6: coefficient = 50; break;
                        default: coefficient = 0; break;
                    }
                    List<WorkingAt> jobs = db.WorkingAts.Where(x => x.UserId == id).ToList();
                    int[] technologies = jobs.Select(x => x.TechId).Distinct().ToArray();
                    int numberOfTechs = technologies.Count();
                    for (int i = 0; i < numberOfTechs; i++)
                    {
                        int months = 0;
                        foreach (WorkingAt job in jobs)
                        {
                            if (job.TechId == technologies[i])
                            {
                                TimeSpan time = new TimeSpan();

                                if (job.Until.HasValue)
                                {
                                    time = job.Until.Value - job.Since;
                                }
                                else
                                {
                                    time = DateTime.Now - job.Since;
                                }

                                months += time.Days / 30;
                            }
                        }

                        salary += ((float)months / 12) * coefficient;
                    }

                    return salary;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static WorkingAtVM[] ReturnJobs(int id)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                WorkingAt[] listToConvert = db.WorkingAts.ToList().Where(x => x.UserId == id).ToArray();
                WorkingAtVM[] list = null;

                if (listToConvert.Count() > 0)
                {
                    int numberOfJobs = 1;
                    for (int i = 1; i < listToConvert.Count(); i++)
                    {
                        if (listToConvert[i].CompanyName != listToConvert[i - 1].CompanyName
                            && listToConvert[i].Since != listToConvert[i - 1].Since)
                        {
                            numberOfJobs++;
                        }
                    }

                    list = new WorkingAtVM[numberOfJobs];

                    list[0] = new WorkingAtVM(listToConvert[0]);
                    int j = 1;

                    for (int i = 1; i < listToConvert.Count(); i++)
                    {
                        if (listToConvert[i].CompanyName != listToConvert[i-1].CompanyName
                            && listToConvert[i].Since != listToConvert[i-1].Since)
                        {
                            list[j] = new WorkingAtVM(listToConvert[i]);
                            j++;
                        }
                    }
                }

                return list;
            }
        }
    }
}