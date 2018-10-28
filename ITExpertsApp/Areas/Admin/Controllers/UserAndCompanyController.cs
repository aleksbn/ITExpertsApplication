using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace ITExpertsApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Mod")]
    public class UserAndCompanyController : Controller
    {
        // GET: Admin/UserAndCompany
        public ActionResult Index()
        {
            List<UserVM> list = new List<UserVM>();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                list = db.Users.ToList().Where(x => x.RoleId == 102 || x.RoleId == 103).Select(x => new UserVM(x)).ToList();
            }

            return View(list);
        }

        [HttpGet]
        public ActionResult ChangeStatus(int id, string status)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.Find(id);

                Company cmp = null;

                if (usr.RoleId == 103)
                {
                    cmp = db.Companies.Single(x => x.Email == usr.Email);
                }

                if (usr != null)
                {
                    if (status == "true")
                    {
                        usr.Blocked = false;

                        if (usr.RoleId == 103)
                        {
                            cmp.Blocked = false;
                        }

                        if (usr.MadeUp == false)
                        {
                            SendMail(usr.Email, 2, "");
                        }
                    }
                    else
                    {
                        usr.Blocked = true;

                        if (usr.RoleId == 103)
                        {
                            cmp.Blocked = true;
                        }

                        if (usr.MadeUp == false)
                        {
                            SendMail(usr.Email, 1, "");
                        }
                    }

                    db.SaveChanges();

                    ViewBag.Result = "The data have been changed.";
                }
            }

            return Redirect("~/Admin/UserAndCompany/Index");
        }

        [HttpGet]
        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User u = db.Users.Find(id);

                if (u != null)
                {
                    UserVM usr = new UserVM(u);

                    return View(usr);
                }
                else
                {
                    ViewBag.Result = "That name does not exist.";
                    return View();
                }
            }
        }

        [HttpPost, ActionName("DeleteUser")]
        public ActionResult DeleteConfirmed(int id, string reason)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.Find(id);

                string mail = usr.Email;

                if (usr != null)
                {
                    db.Users.Remove(usr);

                    if (usr.RoleId == 103)
                    {
                        Company cmp = db.Companies.Single(x => x.Email == usr.Email);

                        db.Companies.Remove(cmp);
                    }

                    if (usr.MadeUp == false)
                    {
                        SendMail(mail, 3, reason);
                    }
                    db.SaveChanges();
                    TempData["Status"] = "User/company deleted";

                    return Redirect("~/Admin/UserAndCompany/Index");
                }
            }

            ViewBag.Result = "The user was not deleted. Something went wrong.";
            return View();
        }

        private void SendMail(string mail, int type, string reason)
        {
            MailAddress sender = new MailAddress("aleksbn417@gmail.com", "Lead administrator from ITExperts.com");
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
                if (usr.MadeUp != true)
                {
                    //admin ili mod blokirao korisnika ili kompaniju, mail ide blokiranom
                    if (type == 1)
                    {
                        msg.Subject = "Your account has been blocked!";

                        if (usr.RoleId == 102)
                        {
                            msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>";
                        }
                        else
                        {
                            msg.Body = "Dear " + usr.FirstName + ",<br>";
                        }

                        msg.Body += "your account has been blocked! If you decide to apeal, do so in a form " +
                            "of reply to this mail.<br><br>Kind regards,<br>Aleksandar Matic";
                    }
                    //admin ili mod odblokirao korisnika ili kompaniju, mail ide odblokiranom
                    else if (type == 2)
                    {
                        msg.Subject = "You got your account back!";

                        if (usr.RoleId == 102)
                        {
                            msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>";
                        }
                        else
                        {
                            msg.Body = "Dear " + usr.FirstName + ",<br>";
                        }

                        msg.Body += "your account has been unblocked. We do hope we won't be taking this course " +
                            "of actions ever again.<br><br>Kind regards,<br>Aleksandar Matic";
                    }
                    //admin ili mod obrisao korisnika ili kompaniju, mail ide obrisanom
                    else if (type == 3)
                    {
                        msg.Subject = "Your account has been permanently deleted!";

                        if (usr.RoleId == 102)
                        {
                            msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>";
                        }
                        else
                        {
                            msg.Body = "Dear " + usr.FirstName + ",<br>";
                        }

                        msg.Body += "your account has been permanently deleted. There is nothing you can do " +
                            "at this time. The reason for deletio is:<br>" + reason +
                            "<br><br>You are free to make a new account if you would like.<br><br>Kind" +
                            " regards,<br>Aleksandar Matic";
                    }

                    client.Send(msg);
                }
            }
        }
    }
}