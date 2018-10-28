using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels.Account;
using ITExpertsApp.Models.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace ITExpertsApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("SearchEmployees");
        }

        [HttpGet]
        public JsonResult SingleUser()
        {
            UserVM user = new UserVM();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                string mail = User.Identity.Name;

                foreach (User usr in db.Users)
                {
                    if (mail.Equals(usr.Email))
                    {
                        user = new UserVM(usr);
                    }
                }
            }

            if (user != null)
            {
                return Json(user, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public JsonResult SingleUserForRespond(int id)
        {
            UserVM user = new UserVM();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                user = new UserVM(db.Users.Find(id));
            }

            if (user != null)
            {
                return Json(user, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [Authorize(Roles = "Mod, Admin, Company")]
        [HttpGet]
        public ActionResult PublicProfile(int id)
        {
            if (id < 100)
            {
                return HttpNotFound();
            }

            UserVM model = null;

            try
            {
                using (ITExpertsContext db = new ITExpertsContext())
                {
                    model = new UserVM(db.Users.Find(id));

                    List<WorkingAt> jobs = db.WorkingAts.Where(x => x.UserId == id).ToList();

                    int index = 0;

                    if (jobs.Count() != 0)
                    {
                        model.Jobs = new WorkingAtVM[db.WorkingAts.Where(x => x.UserId == model.UserId).GroupBy(x => new { x.CompanyName, x.Since }).Select(x => x).Count()];

                        int i = 0;
                        while (i < jobs.Count)
                        {
                            WorkingAtVM job = new WorkingAtVM();

                            job.CompanyId = jobs[i].CompanyId;
                            job.CompanyName = jobs[i].CompanyName;
                            job.Description = jobs[i].Description;
                            job.JobId = jobs[i].JobId;
                            job.PathId = jobs[i].Tech.PathId;
                            job.Since = jobs[i].Since;
                            job.Until = jobs[i].Until;
                            job.UserId = id;
                            job.TechId = new int[db.WorkingAts.Count(x => x.UserId == id && x.CompanyId == job.CompanyId && x.Since == job.Since)];
                            job.Technology = new string[db.WorkingAts.Count(x => x.UserId == id && x.CompanyId == job.CompanyId && x.Since == job.Since)];
                            int j = 0;
                            while (j < job.TechId.Count())
                            {
                                job.TechId[j] = jobs[i].TechId;
                                Technology bla = db.Technologies.Find(jobs[i].TechId);
                                job.Technology[j] = bla.TechName;
                                i++;
                                j++;
                            }
                            model.Jobs[index] = job;
                            index++;
                        }
                    }

                    model.Salary = HelpClass.ReturnSalary(model.UserId);
                }
            }
            catch (Exception)
            {

            }

            if (model != null)
            {
                return View(model);
            }

            return HttpNotFound();
        }

        [Authorize(Roles = "Mod, Admin, Company")]
        [HttpGet]
        public ActionResult SearchEmployees()
        {
            return View();
        }

        [Authorize(Roles = "Mod, Admin, Company")]
        public JsonResult ReturnEmployees(SearchUser criteria)
        {
            List<UserVM> list = new List<UserVM>();
            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                try
                {
                    list = db.Users.Where(x => x.RoleId == 102 && x.Active == true && x.Blocked == false).ToList().Select(x => new UserVM(x)).ToList();
                    List<UserVM> toRemoveUsers = new List<UserVM>();
                    Company cmp = db.Companies.FirstOrDefault(x => x.Email.Equals(mail) == true);
                    User currentRequest = db.Users.FirstOrDefault(x => x.Email.Equals(mail) == true);

                    foreach (UserVM u in list)
                    {
                        if (u.Jobs != null && u.Jobs.Count() > 0)
                        {
                            foreach (WorkingAtVM job in u.Jobs)
                            {
                                if (job.CompanyId == cmp.CompanyId && job.Until == null)
                                {
                                    toRemoveUsers.Add(u);
                                }
                            }
                        }
                    }

                    if (toRemoveUsers.Count > 0)
                    {
                        foreach (UserVM u in toRemoveUsers)
                        {
                            list.Remove(u);
                        }
                    }

                    if (criteria.EducationLevel != 0)
                    {
                        list = list.Where(x => x.EducationLevel == criteria.EducationLevel).ToList();
                    }

                    if (criteria.Newcomers)
                    {
                        list = list.Where(x => x.Jobs == null).ToList();
                    }
                    else
                    {
                        if (criteria.UnemployedOnly)
                        {
                            list = list.Where(x => x.Jobs != null && x.Jobs.Last().Until != null).ToList();
                        }

                        else
                        {
                            if (criteria.MaxSalary > 0)
                            {
                                list = list.Where(x => x.Salary < criteria.MaxSalary).ToList();
                            }

                            if (criteria.MinSalary > 0)
                            {
                                list = list.Where(x => x.Salary > criteria.MinSalary).ToList();
                            }

                            if (criteria.DevPath != 0)
                            {
                                List<UserVM> toRemove = new List<UserVM>();
                                foreach (UserVM user in list)
                                {
                                    bool contains = false;
                                    if (user.Jobs != null)
                                    {
                                        foreach (WorkingAtVM job in user.Jobs)
                                        {
                                            if (job.PathId == criteria.DevPath)
                                            {
                                                contains = true;
                                            }
                                        }
                                    }
                                    if (!contains)
                                    {
                                        toRemove.Add(user);
                                    }
                                }
                                if (toRemove.Count() > 0)
                                {
                                    foreach (UserVM user in toRemove)
                                    {
                                        list.Remove(user);
                                    }
                                }
                            }

                            if (criteria.Technologies != null)
                            {
                                List<UserVM> toRemove = new List<UserVM>();
                                foreach (UserVM user in list)
                                {
                                    int validJobs = 0;
                                    foreach (WorkingAtVM job in user.Jobs)
                                    {
                                        for (int i = 0; i < criteria.Technologies.Count(); i++)
                                        {
                                            if (job.TechId.Contains(criteria.Technologies[i]))
                                            {
                                                validJobs++;
                                            }
                                        }
                                    }
                                    if (validJobs == 0)
                                    {
                                        toRemove.Add(user);
                                    }
                                }

                                if (toRemove.Count() > 0)
                                {
                                    foreach (UserVM user in toRemove)
                                    {
                                        list.Remove(user);
                                    }
                                }
                            }

                            if (criteria.YearsOfWork > 0)
                            {
                                List<UserVM> toRemove = new List<UserVM>();
                                foreach (UserVM user in list)
                                {
                                    List<WorkingAtVM> jobsToCalculate = new List<WorkingAtVM>();
                                    int timeInMonths = 0;

                                    if (user.Jobs != null)
                                    {
                                        foreach (WorkingAtVM job in user.Jobs)
                                        {
                                            int[] usedTechs = new int[job.TechId.Count()];
                                            usedTechs = job.TechId.OrderBy(x => x).ToArray();
                                            bool match = true;

                                            if (criteria.DevPath != job.PathId)
                                            {
                                                match = false;
                                            }
                                            else
                                            {
                                                if (criteria.Technologies != null)
                                                {
                                                    for (int i = 0; i < criteria.Technologies.Count(); i++)
                                                    {
                                                        if (!usedTechs.Contains(criteria.Technologies[i]))
                                                        {
                                                            match = false;
                                                        }
                                                    }
                                                }
                                            }

                                            if (match)
                                            {
                                                jobsToCalculate.Add(job);
                                            }
                                        }

                                        foreach (WorkingAtVM job in jobsToCalculate)
                                        {
                                            if (job.Until != null)
                                            {
                                                timeInMonths += job.Until.Value.Subtract(job.Since).Days / 30;
                                            }
                                            else
                                            {
                                                timeInMonths += DateTime.Now.Subtract(job.Since).Days / 30;
                                            }
                                        }

                                        if (timeInMonths < (criteria.YearsOfWork * 12))
                                        {
                                            toRemove.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        toRemove.Add(user);
                                    }
                                }

                                if (toRemove.Count() > 0)
                                {
                                    foreach (UserVM removeIt in toRemove)
                                    {
                                        list.Remove(removeIt);
                                    }
                                }
                            }

                            if (criteria.EducationLevel > 0)
                            {
                                list = list.Where(x => x.EducationLevel == criteria.EducationLevel).ToList();
                            }
                        }
                    }
                }
                catch (Exception xcp)
                {
                    string error = xcp.Message;
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Quit(int id)
        {
            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.First(x => x.Email.Equals(mail));
                Company cmp = db.Companies.Find(id);

                List<WorkingAt> list = null;
                list = db.WorkingAts.ToList().Where(x => x.CompanyId == cmp.CompanyId && x.UserId == usr.UserId && x.Until == null).ToList();

                if (list != null)
                {
                    foreach (WorkingAt job in list)
                    {
                        job.Until = DateTime.Now;
                    }

                    TempData["Status"] = "You decided to quit your current job. We honestly hope it was a wise decision. Good luck!";
                    SendMail(usr.Email, 1, "");
                    db.SaveChanges();

                    if (cmp.MadeUp != true)
                    {
                        SendMail(usr.Email, 2, cmp.Email);
                    }
                }
                else
                {
                    TempData["Status"] = "You do not currently work there!";
                }
            }

            return RedirectToAction("MyProfile", "Account");
        }

        [Authorize(Roles = "Company")]
        [HttpGet]
        public ActionResult Hire(int id)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                UserVM usr = new UserVM(db.Users.Find(id));
                return View(usr);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPost]
        public ActionResult Hire(int id, int[] TechId)
        {
            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.Find(id);
                Company cmp = db.Companies.First(x => x.Email.Equals(mail));

                if (db.WorkingAts.Any(x => x.UserId == id && x.CompanyId == cmp.CompanyId && x.Until == null))
                {
                    TempData["Status"] = "The selected employee is already working at your company!";
                    return RedirectToAction("SearchEmployees", "Account");
                }

                if (usr.MadeUp == true)
                {
                    foreach (int t in TechId)
                    {
                        WorkingAt job = new WorkingAt();
                        job.Since = DateTime.Now;
                        job.Until = null;
                        job.TechId = t;
                        job.UserId = usr.UserId;
                        job.CompanyId = cmp.CompanyId;
                        job.CompanyName = cmp.CompanyName;
                        job.Description = "Automatically generated description";
                        db.WorkingAts.Add(job);
                        db.SaveChanges();
                    }
                }
                else
                {
                    User cmp2 = db.Users.First(x => x.Email.Equals(mail));
                    JobRequest r = new JobRequest();
                    r.DateOfRequest = DateTime.Now;
                    r.SenderId = cmp2.UserId;
                    r.ReceiverId = usr.UserId;
                    r.Status = "New";
                    string techs = "";
                    foreach (int t in TechId)
                    {
                        techs += t.ToString() + ",";
                    }
                    techs.Remove(techs.Length - 1, 1);
                    r.Techs = techs;
                    db.JobRequests.Add(r);
                    db.SaveChanges();
                    SendMail(mail, 3, usr.Email);
                }

                SendMail(mail, 4, "");
            }

            return RedirectToAction("MyCompanyProfile", "Account");
        }

        private void SendMail(string mail, int type, string reason)
        {
            MailAddress sender = new MailAddress("aleksbn417@gmail.com", "Aleksandar Matic from ITExperts.com");
            MailAddress receiver = new MailAddress(mail);
            MailMessage msg = new MailMessage(sender, receiver);
            msg.From = sender;
            msg.To.Add(receiver);
            msg.IsBodyHtml = true;
            NetworkCredential credentials = new NetworkCredential("aleksbn417@gmail.com", "utbnwuvztkbgwpwj", "");
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = credentials;

            User usr = null;
            Company cmp = null;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                usr = db.Users.Single(x => x.Email.Equals(mail));
            }

            if (usr != null)
            {
                //korisnik je dao otkaz, mail ide njemu
                if (type == 1)
                {
                    msg.Subject = "You quit your job, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>" +
                        "you decided to quit your current job. We hope it was for the best." +
                        "<br><br>Best regards,<br>Aleksandar Matic from ITExperts.com";
                }

                //korisnik je dao otkaz, mail ide kompaniji
                if (type == 2)
                {
                    using (ITExpertsContext db = new ITExpertsContext())
                    {
                        cmp = db.Companies.Single(x => x.Email.Equals(reason));
                        msg.To.Clear();
                        msg.To.Add(reason);
                    }

                    msg.Subject = "You just lost an employee, " + cmp.CompanyName + "!";
                    msg.Body = "Dear " + cmp.CompanyName + ",<br>" +
                        "your employee " + usr.FirstName + " " + usr.LastName + "has decided to part ways with you." +
                        " If you think this is a mistake, try mailing the employee and see if you could fix things between you." +
                        "His mail is: " + usr.Email + "." +
                        "<br><br>Best regards,<br>Aleksandar Matic from ITExperts.com";
                }

                //kompanija je poslala zahtjev korisniku, mail ide korisniku
                if (type == 3)
                {
                    using (ITExpertsContext db = new ITExpertsContext())
                    {
                        cmp = db.Companies.Single(x => x.Email.Equals(reason));
                        msg.To.Clear();
                        msg.To.Add(reason);
                    }

                    msg.Subject = "You have a job request, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>" +
                        "you just received a job request from " + cmp.CompanyName + ". Check your requests to decide " +
                        "wether you'll accept it or no." +
                        "<br><br>Best regards,<br>Aleksandar Matic from ITExperts.com";
                }
                //kompanija je poslala zahtjev korisniku, mail ide kompaniji
                if (type == 4)
                {
                    msg.Subject = "You just send the request, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + ",<br>" +
                        "you just send the employment request. We'll send you the mail when the client responds." +
                        "<br><br>Best regards,<br>Aleksandar Matic from ITExperts.com";
                }

                client.Send(msg);
            }
        }
    }
}