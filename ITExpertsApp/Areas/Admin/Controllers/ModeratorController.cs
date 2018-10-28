using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels;

namespace ITExpertsApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ModeratorController : Controller
    {
        // GET: Admin/Moderator
        public ActionResult Index()
        {
            List<ModeratorVM> ModList = new List<ModeratorVM>();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                ModList = db.Users.ToArray().Where(x => x.RoleId == 101).Select(x => new ModeratorVM(x)).ToList();
            }

            return View(ModList);
        }

        //GET: Admin/Moderator/Create
        [HttpGet]
        public ActionResult AddModerator()
        {
            return View();
        }

        //POST: Admin/Moderator/Create
        [HttpPost]
        public ActionResult AddModerator(ModeratorVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Result = null;
                return View("AddModerator", model);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "The password and the confirmation do not match!");
                return View("AddModerator", model);
            }

            using (ITExpertsContext db = new ITExpertsContext())
            {
                if (db.Users.Any(x => x.Email.Equals(model.Email)))
                {
                    ModelState.AddModelError("", "That email has already been taken!");
                    ViewBag.Result = null;
                    return View("AddModerator", model);
                }

                User newMod = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password,
                    Email = model.Email,
                    RoleId = 101,
                    Active = true,
                    Blocked = false
                };

                db.Users.Add(newMod);
                db.SaveChanges();
            }

            ViewBag.Result = "The new moderator was added.";

            return Redirect("~/Admin/Moderator/Index");
        }

        //GET: Admin/Moderator/ChangeStatus
        [HttpGet]
        public ActionResult ChangeStatus(int id, string status)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                User mod = db.Users.Find(id);

                if (status == "true")
                {
                    mod.Blocked = false;
                }
                else
                {
                    mod.Blocked = true;
                }

                ViewBag.Result = "The new moderator " + mod.FirstName + " " + mod.LastName + "'s status is changed.";
                if (mod.MadeUp == false)
                {
                    SendMail(mod.Email, 2, "");
                }
                db.SaveChanges();
            }

            return Redirect("~/Admin/Moderator/Index");
        }

        //GET: Admin/Moderator/Delete
        [HttpGet]
        public ActionResult DeleteModerator(int? id)
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
                    ModeratorVM mod = new ModeratorVM(u);

                    return View(mod);
                }
                else
                {
                    ViewBag.Result = "That moderator does not exist.";
                    return View();
                }
            }
        }

        //GET: Admin/Moderator/Delete
        [HttpPost, ActionName("DeleteModerator")]
        public ActionResult DeleteConfirmed(int id, string reason)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                User u = db.Users.Find(id);
                db.Users.Remove(u);
                db.SaveChanges();
                ViewBag.Result = "The moderator " + u.FirstName + " " + u.LastName + " was deleted.";
                if (u.MadeUp == false)
                {
                    SendMail(u.Email, 1, reason);
                }
                return Redirect("~/Admin/Moderator/Index");
            }
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
                    //admin obrisao moda, salje se modu
                    if (type == 1)
                    {
                        msg.Subject = "Your account has been permanently deleted!";
                        msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>" +
                            "your account has been permanently deleted! <br><br>Reason:<br>" +
                            reason + "<br><br>If you decide to apeal, do so in a form of reply to this mail." +
                            "<br><br>Kind regards,<br>Aleksandar Matic";
                    }
                    //admin blokirao moda, salje se modu
                    else if (type == 2)
                    {
                        msg.Subject = "Your account has been blocked by administrator!";
                        msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>" +
                            "your account has been blocked! If you decide to apeal, do so in a form of " +
                            "reply to this mail.<br><br>Kind regards,<br>Aleksandar Matic";
                    }

                    client.Send(msg);
                }
            }
        }
    }
}