using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ITExpertsApp.Models.Data;

namespace ITExpertsApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            if (User == null)
            {
                return;
            }

            string mail = Context.User.Identity.Name;

            string[] roles = null;

            using (ITExpertsContext db = new ITExpertsContext())
            {
                User user = db.Users.FirstOrDefault(x => x.Email == mail);

                roles = db.Roles.Where(x => x.RoleId == user.RoleId).Select(x => x.RoleName).ToArray();
            }

            IIdentity userIdentity = new GenericIdentity(mail);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            Context.User = newUserObj;
        }
    }
}
