using ITExpertsApp.Models.Data;
using ITExpertsApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITExpertsApp.Areas.Admin.Controllers
{
    public class TechnologyController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Path()
        {
            return View();
        }

        public JsonResult AllPaths()
        {
            List<DevPathVM> lista = new List<DevPathVM>();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                lista = db.DevPaths.ToArray().Select(x => new DevPathVM(x)).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Admin")]
        public int AddPath(DevPathVM model)
        {
            DevPath row = new DevPath()
            {
                PathId = model.PathId,
                PathName = model.PathName
            };

            using (ITExpertsContext db = new ITExpertsContext())
            {
                try
                {
                    db.DevPaths.Add(row);
                    db.SaveChanges();
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        [Authorize(Roles = "Admin")]
        public int UpdatePath(DevPathVM model)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                DevPath row = db.DevPaths.Find(model.PathId);

                if (row != null)
                {
                    row.PathName = model.PathName;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Authorize(Roles = "Admin")]
        public int DeletePath(int id)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                DevPath row = db.DevPaths.Find(id);

                if (row != null)
                {
                    db.DevPaths.Remove(row);
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public ActionResult Tech()
        {
            return View();
        }

        public JsonResult AllTechs(int pathId)
        {
            List<TechnologyVM> lista = new List<TechnologyVM>();

            using (ITExpertsContext db = new ITExpertsContext())
            {
                lista = db.Technologies.ToArray()
                    .Where(x => x.PathId == pathId)
                    .Select(x => new TechnologyVM(x)).ToList();
            }

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReturnTech(int techId)
        {
            if (techId != 0)
            {
                using (ITExpertsContext db = new ITExpertsContext())
                {
                    Technology row = db.Technologies.Find(techId);
                    TechnologyVM model = new TechnologyVM()
                    {
                        TechId = row.TechId,
                        TechName = row.TechName,
                        TechDescription = row.TechDescription,
                        PathId = row.PathId
                    };

                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return null;
            }
        }

        [Authorize(Roles = "Admin")]
        public int AddTech(TechnologyVM model)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                try
                {
                    Technology row = new Technology()
                    {
                        TechName = model.TechName,
                        TechDescription = model.TechDescription,
                        PathId = model.PathId
                    };

                    db.Technologies.Add(row);
                    db.SaveChanges();
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        [Authorize(Roles = "Admin")]
        public int UpdateTech(TechnologyVM model)
        {
            using (ITExpertsContext db = new ITExpertsContext())
            {
                Technology row = db.Technologies.Find(model.TechId);

                if (row != null)
                {
                    row.TechDescription = model.TechDescription;
                    row.TechName = model.TechName;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Authorize(Roles = "Admin")]
        public int DeleteTech(int techId)
        {
            if (techId > 0)
            {
                using (ITExpertsContext db = new ITExpertsContext())
                {
                    try
                    {
                        Technology row = db.Technologies.Find(techId);
                        db.Technologies.Remove(row);
                        db.SaveChanges();
                        return 1;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return 0;
            }
        }
    }
}