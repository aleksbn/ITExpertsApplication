using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels;
using ITExpertsApp.Models.ViewModels.Account;
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
    public class CompanyController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("CompanyList");
        }

        [HttpGet]
        public JsonResult SingleCompany()
        {
            CompanyVM company = new CompanyVM();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                string mail = User.Identity.Name;

                foreach (Company cmp in db.Companies)
                {
                    if (mail.Equals(cmp.Email) && cmp.Active == true && cmp.Blocked == false)
                    {
                        company = new CompanyVM(cmp);
                    }
                }
            }

            if (company != null)
            {
                return Json(company, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [Authorize(Roles = "Mod, Admin, User")]
        [HttpGet]
        public ActionResult PublicCompanyProfile(int id)
        {
            if (id < 100)
            {
                return HttpNotFound();
            }

            CompanyVM model = null;

            try
            {
                using (ITExpertsContext db = new ITExpertsContext())
                {
                    model = new CompanyVM(db.Companies.Where(x => x.Active == true && x.Blocked == false).ToList().First(x => x.CompanyId == id));
                    model.Employees = db.WorkingAts.Where(x => x.Until == null && x.CompanyId == id && x.User.Blocked == false && x.User.Active == true).Select(x => x.UserId).Distinct().Count();
                }
            }
            catch (Exception)
            {

            }

            return View(model);
        }

        [HttpGet]
        public JsonResult AllCompanies()
        {
            List<CompanyVM> lista = new List<CompanyVM>();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                foreach (Company c in db.Companies)
                {
                    if (c.Active == true && c.Blocked == false)
                    {
                        lista.Add(new CompanyVM(c));
                    }
                }
            }

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AllEmployees()
        {
            string mail = User.Identity.Name;

            Company cmp = null;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                cmp = db.Companies.Single(x => x.Email.Equals(mail));
            }

            if (cmp != null)
            {
                return View(cmp.CompanyId);
            }

            return View(0);
        }

        [HttpGet]
        public JsonResult Employees()
        {
            string mail = User.Identity.Name;

            List<UserVM> list = null;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                Company cmp = db.Companies.Single(x => x.Email == mail);
                List<int> ids = db.WorkingAts.Where(x => x.CompanyId == cmp.CompanyId && x.Until == null).Select(x => x.UserId).Distinct().ToList();

                list = db.Users.Where(x => ids.Contains(x.UserId)).ToList().Select(x => new UserVM(x)).ToList();
            }

            foreach (UserVM user in list)
            {
                user.Salary = Math.Round(user.Salary, 2);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CompanyList()
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                List<CompanyVM> list = db.Companies.ToList().Select(x => new CompanyVM(x)).ToList();
                return View(list);
            }
        }

        [Authorize(Roles = "Company")]
        public JsonResult FireEmployee(int userId, int companyId, string reason)
        {
            try
            {
                using (ITExpertsContext db = new ITExpertsContext())
                {
                    foreach (WorkingAt job in db.WorkingAts.Where(x => x.CompanyId == companyId && x.UserId == userId && x.Until == null).ToList())
                    {
                        job.Until = DateTime.Now;
                    }

                    User usr = db.Users.Find(userId);

                    if (usr.MadeUp != true)
                    {
                        SendMail(usr.Email, 1, reason);
                    }

                    Company cmp = db.Companies.Find(companyId);

                    if (cmp.MadeUp != true)
                    {
                        reason = usr.FirstName + " " + usr.LastName;
                        SendMail(cmp.Email, 2, reason);
                    }

                    db.SaveChanges();
                    
                }

                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }           
        }

        [Authorize(Roles = "User")]
        public ActionResult RequestJob(int id)
        {
            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.FirstOrDefault(x => x.Email.Equals(mail) == true);
                Company cmp = db.Companies.Find(id);
                if (db.WorkingAts.FirstOrDefault(x => x.CompanyId == cmp.CompanyId && x.UserId == usr.UserId && x.Until == null) != null)
                {
                    TempData["Status"] = "You already work here, you can't ask for this job.";
                    return RedirectToAction("MyProfile", "Account");
                }

                if (cmp.MadeUp == true)
                {
                    WorkingAt job = new WorkingAt
                    {
                        Since = DateTime.Now,
                        Until = null,
                        CompanyId = cmp.CompanyId,
                        CompanyName = cmp.CompanyName,
                        Description = "",
                        UserId = usr.UserId,
                        TechId = 101,
                        Tech = db.Technologies.Find(101),
                        User = db.Users.Find(usr.UserId),
                        Company = db.Companies.Find(cmp.CompanyId)
                    };
                    WorkingAtVM jobToSend = new WorkingAtVM(job)
                    {
                        PathId = 101,
                        TechId = new int[1]
                    };
                    jobToSend.TechId[0] = 101;
                    jobToSend.Technology = new string[1];
                    jobToSend.Technology[0] = "Ruby";
                    db.WorkingAts.Add(job);
                    db.SaveChanges();

                    return RedirectToAction("EditHistory", "Account", jobToSend);
                }

                else
                {
                    JobRequest rqst = new JobRequest();
                    rqst.DateOfRequest = DateTime.Now;
                    rqst.SenderId = usr.UserId;
                    rqst.ReceiverId = db.Users.First(x => x.Email.Equals(cmp.Email)).UserId;
                    rqst.Status = "New";

                    SendMail(cmp.Email, 3, "");

                    TempData["Status"] = "You sent the request. You won't be able to send another requests to any company for another week, or until the company responds.";

                    return RedirectToAction("MyProfile", "Account");
                }
            }
        }

        [Authorize(Roles = "Company")]
        public JsonResult CheckIfEmployed(int UserId)
        {
            string CompanyMail = User.Identity.Name;
            using (ITExpertsContext db = new ITExpertsContext())
            {
                Company cmp = db.Companies.First(x => x.Email.Equals(CompanyMail));

                if (db.WorkingAts.Any(x => x.UserId == UserId && x.CompanyId == cmp.CompanyId && x.Until == null))
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
            }
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

            using (ITExpertsContext db = new ITExpertsContext())
            {
                usr = db.Users.Single(x => x.Email.Equals(mail));
            }

            if (usr != null)
            {
                //korisnik je otpusten, mail ide njemu
                if (type == 1)
                {
                    msg.Subject = "You have been fired, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>" +
                        "it seems your current employer has fired you. We would suggest you send them email and see " +
                        "if there's a possibility for some mistake. Their reason:<br>" + reason +
                        "<br><br>Best regards,<br>Aleksandar Matic from ITExperts.com";
                }
                //korisnik je otpusten, mail ide kompaniji
                if (type == 2)
                {
                    msg.Subject = "You fired someone, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + ",<br>" +
                        "this is just a notification email to inform you that you fired a worker " + reason + "." +
                        "<br><br>Best regards,<br>Aleksandar Matic";
                }
                //korisnik je poslao zahtjev za posao, mail ide kompaniji
                if (type == 3)
                {
                    msg.Subject = "You have a job application, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + ",<br>" +
                        "it seems someone requested a working spot in your company. Please, log in to review the request." +
                        "<br><br>Best regards,<br>Aleksandar Matic";
                }

                client.Send(msg);
            }
        }
    }
}