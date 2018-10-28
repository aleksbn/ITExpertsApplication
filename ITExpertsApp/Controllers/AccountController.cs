using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.Help;
using ITExpertsApp.Models.ViewModels;
using ITExpertsApp.Models.ViewModels.Account;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ITExpertsApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }

        // GET: /account/createaccount
        [AllowAnonymous]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: /account/createuseraccount
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateUserAccount(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                Session["Message"] = "Something is not right with your application! Check if you filled all data.";
                return View("CreateAccount", model);
            }

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User row = new User();

                row.Active = true;
                row.Email = model.UserEmail;
                row.FirstName = model.FirstName;
                row.LastName = model.LastName;
                row.Password = model.UserPassword;
                row.RoleId = 102;
                row.Blocked = false;
                row.IsRegistered = false;
                row.RegistrationString = GenerateRegString();
                row.MadeUp = false;

                if (db.Users.Any(x => x.Email.Equals(model.UserEmail)))
                {
                    ModelState.AddModelError("", "That email has already been taken!");
                    Session["Message"] = "That email has already been taken.";
                    return View("CreateAccount", model);
                }
                
                db.Users.Add(row);
                db.SaveChanges();
                SendMail(row.Email, 1);
                FormsAuthentication.SetAuthCookie(row.Email, false);

                Session["Message"] = "You have created your account. Now, login and add some more data about yourself!";
            }

            return RedirectToAction("Welcome", "Account", model.UserEmail);
        }

        // POST: /account/createcompanyaccount
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateCompanyAccount(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                Session["Message"] = "Something is not right with your application! Check if you filled all data.";
                return View("CreateAccount", model);
            }

            using (ITExpertsContext db = new ITExpertsContext())
            {
                Company row = new Company();

                if (db.Users.Any(x => x.Email.Equals(model.UserEmail)))
                {
                    ModelState.AddModelError("", "That email has already been taken!");
                    Session["Message"] = "That email has already been taken.";
                    return View("CreateAccount", model);
                }

                row.CompanyName = model.CompanyName;
                row.Email = model.CompanyEmail;
                row.Password = model.CompanyPassword;
                row.Active = true;
                row.IsCompleted = false;
                row.Blocked = false;
                row.MadeUp = false;
                db.Companies.Add(row);
                db.SaveChanges();

                User usr = new User();

                usr.FirstName = row.CompanyName;
                usr.LastName = row.CompanyName;
                usr.Password = row.Password;
                usr.Active = true;
                usr.Email = row.Email;
                usr.IsCompleted = row.IsCompleted;
                usr.RoleId = 103;
                usr.Blocked = false;
                usr.IsRegistered = false;
                usr.RegistrationString = GenerateRegString();
                row.MadeUp = false;
                db.Users.Add(usr);
                db.SaveChanges();

                SendMail(row.Email, 2);

                Session["Message"] = "You have created the account for your company. Now, login and search for some employees!";
            }

            return RedirectToAction("Welcome", "Account", model.UserEmail);
        }

        // GET: account/login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            string username = User.Identity.Name;
            Session["Option"] = null;

            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: account/login
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (ITExpertsContext db = new ITExpertsContext())
            {
                List<User> userList = db.Users.ToList();
                
                foreach (var usr in userList)
                {
                    if (usr.Email.Equals(model.Email) && usr.Password.Equals(model.Password))
                    {
                        if (usr.Blocked == false)
                        {
                            if (usr.IsRegistered == true)
                            {
                                FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);

                                if (usr.Active == false)
                                {
                                    usr.Active = true;
                                    SendMail(usr.Email, 6);
                                }
                                
                                db.SaveChanges();

                                if (usr.RoleId == 100)
                                {
                                    return RedirectToAction("Index", "Moderator", new { area = "Admin" });
                                }
                                else if (usr.RoleId == 101)
                                {
                                    return RedirectToAction("Index", "UserAndCompany", new { area = "Admin" });
                                }
                                else if (usr.RoleId == 102)
                                {
                                    return RedirectToAction("/MyProfile");
                                }
                                else if (usr.RoleId == 103)
                                {
                                    Company cmp = db.Companies.First(x => x.Email == model.Email);
                                    cmp.Active = true;
                                    db.SaveChanges();
                                    return RedirectToAction("/MyCompanyProfile");
                                }
                            }
                            else
                            {
                                ViewBag.NotRegistered = "1";
                                ViewBag.BadLogin = "You did not complete your account! Check yoir mail for registration link" +
                                    " and click on it. If it's not in your inbox, try the spam folder or trash. If all fails, try " +
                                    "resending the link by using the link below.";
                                return View(model);
                            }
                        }
                        else
                        {
                            ViewBag.BadLogin = "Your account has been blocked! If you believe you've been" +
                                " banned for no reason, write the moderator on aleksbn417@hotmail.com address.";
                            return View(model);
                        }
                    }
                }
            }

            ViewBag.BadLogin = "Wrong username or password!";
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect("~/Account/Login");
        }

        //GET: account/accprofile
        [HttpGet]
        public ActionResult Welcome()
        {
            string email = Request.QueryString.Get("mail");
            return View(email);
        }

        //GET: account/finishprofile
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult FinishProfile()
        {
            return View();
        }

        //POST: account/finishprofile
        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult FinishProfile(UserVM model)
        {
            string mail = User.Identity.Name;

            if (!ModelState.IsValid)
            {
                foreach (ModelState modelState in ModelState.Values)
                {
                    ViewBag.Error = "";
                    StringBuilder sb = new StringBuilder();

                    foreach (ModelError error in modelState.Errors)
                    {
                        sb.AppendLine(error.ErrorMessage);
                    }

                    ViewBag.Error = sb.ToString();
                }
                return View(model);
            }

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User u = db.Users.Single(x => x.Email.Equals(mail));

                if (u == null || u.IsCompleted == true)
                {
                    return RedirectToAction("Index");
                }

                foreach (User usr in db.Users)
                {
                    if (usr.Email.Equals(mail))
                    {
                        usr.Active = true;
                        usr.AverageGrade = model.AverageGrade;
                        usr.DateOfBirth = model.DateOfBirth;
                        usr.Description = model.Description;
                        usr.EducationLevel = model.EducationLevel;
                        usr.IsCompleted = true;
                        usr.LinkedIn = model.LinkedIn;
                        usr.RoleId = 102;
                        usr.StackOverflow = model.StackOverflow;
                        usr.Title = model.Title;
                        usr.MadeUp = false;
                    }
                }

                db.SaveChanges();
            }

            FormsAuthentication.SignOut();

            return RedirectToAction("AllExperience");
        }

        //GET: account/finishcompany
        [Authorize(Roles = "Company")]
        [HttpGet]
        public ActionResult FinishCompany()
        {
            return View();
        }

        //POST: account/finishcompany
        [Authorize(Roles = "Company")]
        [HttpPost]
        public ActionResult FinishCompany(CompanyVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Your data are not valid.";
                return View(model);
            }

            using (ITExpertsContext db = new ITExpertsContext())
            {
                string mail = User.Identity.Name;

                Company c = db.Companies.Single(x => x.Email.Equals(mail));

                if (c == null || c.IsCompleted == true)
                {
                    return RedirectToAction("Index");
                }

                foreach (Company cmp in db.Companies)
                {
                    if (cmp.Email.Equals(model.Email))
                    {
                        cmp.Active = true;
                        cmp.Budget = model.Budget;
                        cmp.CompanyName = model.CompanyName;
                        cmp.Description = model.Description;
                        cmp.Email = model.Email;
                        cmp.IsCompleted = true;
                        cmp.MadeUp = false;
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult AllExperience()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult AllExperience(WorkingAtVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Result = "There have been some mistakes in your form. Correct them, please!";
                return View(model);
            }

            string user = User.Identity.Name;
            EditUserVM userToSend = null;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User u = db.Users.FirstOrDefault(x => x.Email.Equals(user) == true);
                userToSend = new EditUserVM(u);

                if (model.CompanyName != null)
                {
                    model.CompanyId = 0;
                }
                else
                {
                    model.CompanyName = db.Companies.FirstOrDefault(x => x.CompanyId == model.CompanyId).CompanyName;
                }
                
                foreach (int id in model.TechId)
                {
                    WorkingAt job = new WorkingAt {
                        CompanyId = model.CompanyId,
                        CompanyName = model.CompanyName,
                        Description = model.Description,
                        Since = model.Since,
                        Until = model.Until,
                        TechId = id,
                        UserId = u.UserId
                    };

                    db.WorkingAts.Add(job);
                    db.SaveChanges();
                }
            }

            TempData["Status"] = "You added your job history.";
            return RedirectToAction("MyProfile");
        }

        public JsonResult TimeCheck()
        {
            string time = Request.QueryString.ToString();
            int rez = 0;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                int id = db.Users.FirstOrDefault(x => x.Email.Equals(User.Identity.Name)).UserId;

                List<WorkingAt> currentHistory = db.WorkingAts.Where(x => x.UserId == id).ToList();

                TimeFrame frame = new TimeFrame();

                frame.Since = DateTime.Parse(time.Split('&')[0].Split('=')[1]);
                if (time.Split('&')[1].Split('=')[1] != "")
                {
                    frame.Until = DateTime.Parse(time.Split('&')[1].Split('=')[1]);
                }
                else
                {
                    frame.Until = null;
                }

                if (frame.Until != null && frame.Since > frame.Until)
                {
                    DateTime? temp = new DateTime();
                    temp = frame.Until;
                    frame.Until = frame.Since;
                    frame.Since = (DateTime)temp;
                }

                foreach (WorkingAt w in currentHistory)
                {
                    DateTime start = w.Since;
                    DateTime? stop = w.Until;

                    if (stop != null && frame.Until != null)
                    {
                        if ((frame.Since > start && frame.Since < stop)
                        || (frame.Until > start && frame.Until < stop)
                        || (frame.Since < start && frame.Until > stop))
                        {
                            rez = 1;
                        }
                    }
                    else if (frame.Until == null)
                    {
                        if (frame.Since < stop)
                        {
                            rez = 1;
                        }
                    }
                    else
                    {
                        if (frame.Until > start)
                        {
                            rez = 1;
                        }
                    }
                }
            }

            return Json(rez, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "User")]
        public JsonResult DeleteHistory(int userId, int companyId)
        {
            string email = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User currentUser = db.Users.FirstOrDefault(x => x.Email.Equals(email));

                if (currentUser.UserId == userId)
                {
                    List<WorkingAt> list = db.WorkingAts.Where(x => x.UserId == userId && x.CompanyId == companyId).ToList();

                    foreach (WorkingAt item in list)
                    {
                        db.WorkingAts.Remove(item);
                    }

                    db.SaveChanges();

                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public JsonResult GetAllHistory()
        {
            string email = User.Identity.Name;
            User user = new User();
            List<WorkingAt> listForConvert = new List<WorkingAt>();
            List<WorkingAtVM> convertedList = new List<WorkingAtVM>();


            using (ITExpertsContext db = new ITExpertsContext())
            {
                user = db.Users.FirstOrDefault(x => x.Email.Equals(email));

                listForConvert = db.WorkingAts.Where(x => x.UserId == user.UserId).ToList();
                convertedList = new List<WorkingAtVM>();
                foreach (WorkingAt job in listForConvert)
                {
                    WorkingAtVM temp = new WorkingAtVM(job);
                    convertedList.Add(temp);
                }
            }

            return Json(convertedList, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult EditHistory(int UserId, int CompanyId)
        {
            WorkingAt model = new WorkingAt();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                model = db.WorkingAts.FirstOrDefault(x => x.UserId == UserId && x.CompanyId == CompanyId);
                WorkingAtVM modelForSending = new WorkingAtVM(model);
                modelForSending.TechId = new int[10];

                for (int i = 0; i < db.WorkingAts.Where(x => x.UserId == UserId && x.CompanyId == CompanyId).ToList().Count; i++)
                {
                    int TechId = db.WorkingAts.Where(x => x.UserId == UserId && x.CompanyId == CompanyId).ToList()[i].TechId;
                    modelForSending.TechId[i] = TechId;
                }

                return View(modelForSending);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult EditHistory(WorkingAtVM model, int OldId)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Status = "Something went wrong!";
                return View(model);
            }

            using (ITExpertsContext db = new ITExpertsContext())
            {
                List<WorkingAt> currentUserJobs = db.WorkingAts.Where(x => x.UserId == model.UserId && x.CompanyId == OldId).Select(x => x).ToList();
                List<WorkingAt> toAddList = new List<WorkingAt>();
                List<WorkingAt> toDeleteList = new List<WorkingAt>();

                //finding those job skills that are not on the list anymore
                foreach (WorkingAt w in currentUserJobs)
                {
                    bool exists = false;

                    foreach (int i in model.TechId)
                    {
                        if (i == w.TechId)
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        toDeleteList.Add(w);
                    }
                }

                //creating new job skill entries
                foreach (int i in model.TechId)
                {
                    bool exists = false;

                    foreach (WorkingAt w in currentUserJobs)
                    {
                        if (i == w.TechId)
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        WorkingAt job = new WorkingAt
                        {
                            CompanyId = model.CompanyId,
                            CompanyName = model.CompanyName,
                            Description = model.Description,
                            Since = model.Since,
                            Until = model.Until,
                            TechId = i,
                            UserId = model.UserId
                        };

                        toAddList.Add(job);
                    }
                }

                //changing current jobs
                foreach (WorkingAt job in currentUserJobs)
                {
                    job.Since = model.Since;
                    job.Until = model.Until;
                    job.CompanyName = model.CompanyName;
                    job.Description = model.Description;
                    job.CompanyId = model.CompanyId;
                }

                //updating database
                foreach (WorkingAt job in toAddList)
                {
                    db.WorkingAts.Add(job);
                }
                foreach (WorkingAt job in toDeleteList)
                {
                    db.WorkingAts.Remove(job);
                }

                db.SaveChanges();
            }
            return RedirectToAction("AllExperience");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult MyProfile()
        {
            string user = User.Identity.Name;
            TempData["Edit"] = "0";

            User currentUser = new User();
            EditUserVM model = null;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                try
                {
                    currentUser = db.Users.FirstOrDefault(x => x.Email.Equals(user) == true);
                    model = new EditUserVM(currentUser);
                }
                catch (Exception)
                {

                }
            }

            if (currentUser != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult MyProfile(EditUserVM model)
        {
            if (ModelState.IsValid)
            {
                int changeSuccess = 0;

                if (model.OldPassword != null || model.Password != null || model.ConfirmPassword != null)
                {
                    //Changing password aswell
                    try
                    {
                        using (ITExpertsContext db = new ITExpertsContext())
                        {
                            User changedUser = db.Users.Find(model.UserId);

                            if (model.OldPassword.Equals(changedUser.Password))
                            {
                                if ((model.Password.Length >= 6 && model.Password.Length <= 50) &&
                                    (model.ConfirmPassword.Length >= 6 && model.ConfirmPassword.Length <= 50) &&
                                    (model.ConfirmPassword.Equals(model.Password)))
                                {
                                    changedUser.Password = model.Password;
                                    EditSingleUser(changedUser, model);

                                    db.SaveChanges();
                                    changeSuccess = 1;
                                }
                                else
                                {
                                    TempData["Status"] = "Rules for the password change:\n" +
                                        "Both password and the confirmation must have minimum of 6 " +
                                        "and maximum of 50 letters. Password and confirmation must match!";
                                    TempData["Edit"] = "0";
                                    return View(model);
                                }
                            }
                            else
                            {
                                TempData["Status"] = "Old password is wrong!";
                                TempData["Edit"] = "0";
                                return View(model);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    try
                    {
                        using (ITExpertsContext db = new ITExpertsContext())
                        {
                            User changedUser = db.Users.Find(model.UserId);
                            EditSingleUser(changedUser, model);

                            db.SaveChanges();
                            changeSuccess = 1;
                        }
                    }
                    catch (Exception)
                    {

                    }
                    
                }

                if (changeSuccess == 1)
                {
                    TempData["Status"] = "Change succeded!";
                    TempData["Edit"] = "1";
                    model.OldPassword = null;
                    model.ConfirmPassword = null;
                    model.Password = null;
                    FormsAuthentication.SignOut();
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                }
            }

            return View(model);
        }

        private void EditSingleUser(User changedUser, EditUserVM model)
        {
            changedUser.AverageGrade = model.AverageGrade;
            changedUser.DateOfBirth = model.DateOfBirth;
            changedUser.Description = model.Description;
            changedUser.EducationLevel = model.EducationLevel;
            changedUser.Email = model.Email;
            changedUser.FirstName = model.FirstName;
            changedUser.LastName = model.LastName;
            changedUser.LinkedIn = model.LinkedIn;
            changedUser.StackOverflow = model.StackOverflow;
            changedUser.Title = model.Title;
        }

        private void EditSingleCompany(Company changedCompany, User changedUser, EditCompanyVM model)
        {
            changedUser.Description = model.Description;
            changedUser.Email = model.Email;
            changedUser.FirstName = model.CompanyName;
            changedUser.LastName = model.CompanyName;
            changedCompany.Description = model.Description;
            changedCompany.Email = model.Email;
            changedCompany.CompanyName = model.CompanyName;
        }

        [HttpGet]
        [Authorize(Roles = "Company")]
        public ActionResult MyCompanyProfile()
        {
            string user = User.Identity.Name;
            TempData["Edit"] = "0";

            Company currentCompany = new Company();
            EditCompanyVM model = null;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                try
                {
                    currentCompany = db.Companies.FirstOrDefault(x => x.Email.Equals(user) == true);
                    model = new EditCompanyVM(currentCompany);
                }
                catch (Exception)
                {

                }
            }

            if (currentCompany != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Company")]
        public ActionResult MyCompanyProfile(EditCompanyVM model, string UserMail)
        {
            if (ModelState.IsValid)
            {
                int changeSuccess = 0;

                if (model.OldPassword != null || model.Password != null || model.ConfirmPassword != null)
                {
                    //Changing password aswell
                    try
                    {
                        using (ITExpertsContext db = new ITExpertsContext())
                        {
                            Company changedCompany = db.Companies.Find(model.CompanyId);
                            User changedUser = db.Users.Single(x => x.Email == UserMail);

                            if (model.OldPassword.Equals(changedUser.Password))
                            {
                                if ((model.Password.Length >= 6 && model.Password.Length <= 50) &&
                                    (model.ConfirmPassword.Length >= 6 && model.ConfirmPassword.Length <= 50) &&
                                    (model.ConfirmPassword.Equals(model.Password)))
                                {
                                    changedUser.Password = model.Password;
                                    changedCompany.Password = model.Password;
                                    EditSingleCompany(changedCompany, changedUser, model);

                                    db.SaveChanges();
                                    changeSuccess = 1;
                                }
                                else
                                {
                                    TempData["Status"] = "Rules for the password change:\n" +
                                        "Both password and the confirmation must have minimum of 6 " +
                                        "and maximum of 50 letters. Password and confirmation must match!";
                                    TempData["Edit"] = "0";
                                    return View(model);
                                }
                            }
                            else
                            {
                                TempData["Status"] = "Old password is wrong!";
                                TempData["Edit"] = "0";
                                return View(model);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    try
                    {
                        using (ITExpertsContext db = new ITExpertsContext())
                        {
                            Company changedCompany = db.Companies.Find(model.CompanyId);
                            User changedUser = db.Users.Single(x => x.Email == UserMail);

                            EditSingleCompany(changedCompany, changedUser, model);

                            db.SaveChanges();
                            changeSuccess = 1;
                        }
                    }
                    catch (Exception)
                    {

                    }

                }

                if (changeSuccess == 1)
                {
                    TempData["Status"] = "Change succeded!";
                    TempData["Edit"] = "1";
                    model.OldPassword = null;
                    model.ConfirmPassword = null;
                    model.Password = null;
                    FormsAuthentication.SignOut();
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Finish()
        {
            int.TryParse(Request.QueryString.Get("id"), out int id);
            string hash = Request.QueryString.Get("hash");
            User usr = null;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                usr = db.Users.Find(id);

                if (usr != null)
                {
                    if (usr.RegistrationString.Equals(hash))
                    {
                        TempData["Status"] = "You succesfully registered! Login to proceed.";
                        usr.IsRegistered = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        TempData["Status"] = "Something went wrong! Try logging in, please!";
                    }
                }
                else
                {
                    TempData["Status"] = "Apparently, your account does not exist. Try logging again, just in case it's some minor error.";
                }
            }

            if (usr.RoleId == 102)
            {
                return RedirectToAction("FinishProfile");
            }
            if (usr.RoleId == 103)
            {
                return RedirectToAction("FinishCompanyProfile");
            }

            return RedirectToAction("Login");
        }

        private double ReturnSalary(int id)
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

        public PartialViewResult _UserNavPartial()
        {
            UserNavPartialVM model;

            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                model = new UserNavPartialVM();

                if (User.IsInRole("Company"))
                {
                    Company row = db.Companies.FirstOrDefault(x => x.Email == mail);
                    model.Email = row.Email;
                }
                else
                {
                    User row = db.Users.FirstOrDefault(x => x.Email == mail);
                    model.Email = row.Email;
                }
            }

            return PartialView(model);
        }

        private string GenerateRegString()
        {
            char[] s = new char[30];
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random rnd = new Random();

            for (int i = 0; i < 30; i++)
            {
                s[i] = letters[rnd.Next(s.Length)];
            }

            return new string(s);
        }

        [AllowAnonymous]
        public ActionResult Resend(string mail)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.FirstOrDefault(x => x.Email.Equals(mail) == true);

                if (usr.IsRegistered == false)
                {
                    usr.RegistrationString = GenerateRegString();
                    db.SaveChanges();
                    SendMail(mail, 3);
                }
            }

            return RedirectToAction("Login");
        }

        [Authorize(Roles = "User")]
        public ActionResult Deactivate(int id)
        {
            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.FirstOrDefault(x => x.Email.Equals(mail) == true);

                if (usr.UserId != id)
                {
                    TempData["Status"] = "You tried to deactivate someone else's account. Not gonna happen!";
                    return RedirectToAction("Login");
                }

                usr.Active = false;
                db.SaveChanges();
                if (usr.MadeUp == false)
                {
                    SendMail(mail, 4);
                }
                FormsAuthentication.SignOut();
                TempData["Status"] = "You deactivated your account. If you ever decide to use it again, just login!";
                return RedirectToAction("Login", "Account");
            }
        }

        [Authorize(Roles = "User")]
        public ActionResult Delete(int id)
        {
            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.FirstOrDefault(x => x.Email.Equals(mail) == true);

                if (usr.UserId != id)
                {
                    TempData["Status"] = "You tried to delete someone else's account. Not gonna happen!";
                    return RedirectToAction("Login");
                }

                if (usr.MadeUp == false)
                {
                    SendMail(mail, 5);
                }

                FormsAuthentication.SignOut();
                TempData["Status"] = "You deleted permanently your account. If you ever decide to use it again, you'll have to make a new one!";
                db.Users.Remove(usr);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
        }

        [Authorize(Roles = "Company")]
        public ActionResult ComDeactivate(int id)
        {
            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.FirstOrDefault(x => x.Email.Equals(mail) == true);
                Company cmp = db.Companies.FirstOrDefault(x => x.Email.Equals(mail) == true);

                if (cmp.CompanyId != id)
                {
                    TempData["Status"] = "You tried to deactivate someone else's account. Not gonna happen!";
                    return RedirectToAction("Login");
                }

                usr.Active = false;
                cmp.Active = false;
                db.SaveChanges();
                if (usr.MadeUp == false)
                {
                    SendMail(mail, 4);
                }
                FormsAuthentication.SignOut();
                TempData["Status"] = "You deactivated your account. If you ever decide to use it again, just login!";
                return RedirectToAction("Login", "Account");
            }
        }

        [Authorize(Roles = "Company")]
        public ActionResult ComDelete(int id)
        {
            string mail = User.Identity.Name;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User usr = db.Users.FirstOrDefault(x => x.Email.Equals(mail) == true);
                Company cmp = db.Companies.FirstOrDefault(x => x.Email.Equals(mail) == true);

                if (cmp.CompanyId != id)
                {
                    TempData["Status"] = "You tried to delete someone else's account. Not gonna happen!";
                    return RedirectToAction("Login");
                }

                if (usr.MadeUp == false)
                {
                    SendMail(mail, 5);
                }
                FormsAuthentication.SignOut();
                TempData["Status"] = "You deleted permanently your account. If you ever decide to use it again, you'll have to make a new one!";
                db.Users.Remove(usr);
                db.Companies.Remove(cmp);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult RequestList()
        {
            string mail = User.Identity.Name;

            TempData["Mail"] = mail;

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Respond(int answer, int sender, int receiver)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                JobRequest request = db.JobRequests.First(x => x.SenderId == sender && x.ReceiverId == receiver);
                int numberOfJobs = 0;

                if (answer == 0)
                {
                    db.JobRequests.Remove(request);
                    db.SaveChanges();
                }
                else
                {
                    User usr = null;
                    Company cmp = null;

                    if (db.Users.Find(sender).RoleId == 102)
                    {
                        usr = db.Users.Find(sender);
                        User temp = db.Users.Find(receiver);
                        cmp = db.Companies.First(x => x.Email.Equals(temp.Email));
                    }
                    else
                    {
                        usr = db.Users.Find(receiver);
                        User temp = db.Users.Find(sender);
                        cmp = db.Companies.First(x => x.Email.Equals(temp.Email));
                    }

                    WorkingAt job = null;

                    if (request.Techs.Length > 3)
                    {
                        string[] techs = request.Techs.Split(',');
                        foreach (string t in techs)
                        {
                            job = new WorkingAt
                            {
                                Since = DateTime.Now,
                                Until = null,
                                CompanyId = cmp.CompanyId,
                                CompanyName = cmp.CompanyName,
                                Description = "Automatically generated job description",
                                TechId = int.Parse(t),
                                UserId = usr.UserId,
                                Tech = db.Technologies.Find(int.Parse(t)),
                                User = db.Users.Find(usr.UserId),
                                Company = db.Companies.Find(cmp.CompanyId)
                            };
                            db.WorkingAts.Add(job);
                            numberOfJobs++;
                        }
                    }
                    else
                    {
                        job = new WorkingAt
                        {
                            Since = DateTime.Now,
                            Until = null,
                            CompanyId = cmp.CompanyId,
                            CompanyName = cmp.CompanyName,
                            Description = "Automatically generated job description",
                            TechId = 101,
                            UserId = usr.UserId,
                            Tech = db.Technologies.Find(101),
                            User = db.Users.Find(usr.UserId),
                            Company = db.Companies.Find(cmp.CompanyId)
                        };
                        db.WorkingAts.Add(job);
                        numberOfJobs++;
                    }

                    db.JobRequests.Remove(request);
                    db.SaveChanges();

                    if (db.Users.Find(sender).RoleId == 103)
                    {
                        WorkingAtVM jobToSend = new WorkingAtVM(db.WorkingAts.First(x => x.UserId == usr.UserId && x.CompanyId == cmp.CompanyId && x.Until == null));

                        return RedirectToAction("EditHistory", "Account", jobToSend);
                    }
                }
            }

            return View("RequestList");
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetRequests()
        {
            string mail = User.Identity.Name;
            User usr = null;

            List<JobRequest> allRequests = null;
            List<JobRequest> toSend = new List<JobRequest>();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                usr = db.Users.Single(x => x.Email.Equals(mail));
                allRequests = db.JobRequests.Where(x => x.SenderId == usr.UserId || x.ReceiverId == usr.UserId).ToList();

                foreach (JobRequest r in allRequests)
                {
                    toSend.Add(new JobRequest {
                        DateOfRequest = r.DateOfRequest,
                        ReceiverId = r.ReceiverId,
                        SenderId = r.SenderId,
                        Techs = r.Techs,
                        Status = r.Status
                    });
                }

                foreach (JobRequest r in allRequests)
                {
                    if (r.ReceiverId == usr.UserId)
                    {
                        r.Status = "Read";
                    }
                }

                db.SaveChanges();
            }

            return Json(toSend, JsonRequestBehavior.AllowGet);
        }

        private void SendMail(string mail, int type)
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
                //korisnik se registrovao, mail ide korisniku
                if (type == 1)
                {
                    msg.Subject = "Welcome to ITExperts.com, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + " " + usr.LastName + ",<br>" +
                        "on behalf of our entire team, we would like to welcome you to our web service! Our " +
                        "idea here is to connect those who seek IT esperts like yourself, and the creative " +
                        "force of development, you and many ther programmers all over the world! If you have any" +
                        " question, feel free to ask me anything on my question " +
                        "<a href=\"mailto:aleksandar.matic986@gmail.com\">mail</a>, " +
                        "and I'll give my best to get back to you in shorthest time possible. In the meantime, " +
                        "click on this link and finish your registration, please!" +
                        "<br><br><a href=\"https://itexperts.azurewebsites.net/account/finish?id=" + usr.UserId +
                        "&hash=" + usr.RegistrationString +
                        "\">Link for registration</a><br><br>Best regards,<br>Aleksandar Matic";
                }
                //kompanija se registrovala, mail ide kompaniji
                if (type == 2)
                {
                    msg.Subject = "Welcome to ITExperts.com, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + ",<br>" +
                        "on behalf of our entire team, we would like to welcome you to our web service! Our " +
                        "idea here is to connect those who seek IT esperts, like yourself, and the creative " +
                        "force of development, many programmers all over the world! If you have any" +
                        " question, feel free to ask me anything on my question " +
                        "<a href=\"mailto:aleksandar.matic986@gmail.com\">mail</a>, " +
                        "and I'll give my best to get back to you in shorthest time possible. In the meantime, " +
                        "click on this link and finish your registration, please!" +
                        "<br><br><a href=\"https://itexperts.azurewebsites.net/account/finish?id=" + usr.UserId +
                        "&hash=" + usr.RegistrationString +
                        "\">Link for registration</a><br><br>Best regards,<br>Aleksandar Matic";
                }
                //neko je trazio ponovni link za potvrdu registracije
                if (type == 3)
                {
                    msg.Subject = "Here's your new confirmation link, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + ",<br>" +
                        "this is your new registration link, " +
                        "click on it and finish your registration, please!" +
                        "<br><br><a href=\"https://itexperts.azurewebsites.net/account/finish?id=" + usr.UserId +
                        "&hash=" + usr.RegistrationString +
                        "\">Link for registration</a><br><br>Best regards,<br>Aleksandar Matic";
                }
                //kompanija ili korisnik su deaktivirali nalog, mail ide njima
                if (type == 4)
                {
                    msg.Subject = "You deactivated your account, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + ",<br>" +
                        "you are receiving this mail because you deactivated your account. If you want to get it back " +
                        "at any time, just use your mail and password to login and you'll be back in the game!" +
                        "<br><br>Best regards,<br>Aleksandar Matic";
                }
                //kompanija ili korisnik su obrisali nalog, mail ide njima
                if (type == 5)
                {
                    msg.Subject = "You deleted your account, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + ",<br>" +
                        "you are receiving this mail because you deleted your account. We are sorry to see you leave. " +
                        "If you ever decide to get back to us, you'll have to make a new account." +
                        "<br><br>Best regards,<br>Aleksandar Matic";
                }
                //kompanija ili korisnik su ponovo aktivirali svoj nalog, mail ide njima
                if (type == 6)
                {
                    msg.Subject = "You got your account back, " + usr.FirstName + "!";
                    msg.Body = "Dear " + usr.FirstName + ",<br>" +
                        "you are receiving this mail because you activated your account. We wish you welcome back!" +
                        "<br><br>Best regards,<br>Aleksandar Matic";
                }

                client.Send(msg);
            }
        }
    }
}