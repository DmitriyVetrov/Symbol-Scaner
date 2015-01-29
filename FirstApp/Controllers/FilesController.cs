using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScanerApp.Controllers
{
    public class FilesController : Controller
    {
        // GET: Files
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload(ICollection<HttpPostedFileBase> inputfiles)
        {
            if (inputfiles.Count > 0)
            {
                foreach (HttpPostedFileBase file in inputfiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                        file.SaveAs(path);
                    }
                }
                TempData["message"] = string.Format("{0} files has been saved", inputfiles.Count);
            }

            return RedirectToAction("Upload");
        }

        [HttpGet]
        public PartialViewResult GetFiles()
        {
            DirectoryInfo dir_info = new DirectoryInfo(Server.MapPath("~/Uploads/"));
            FileInfo[] files = dir_info.GetFiles();

            return PartialView("_GetFiles", files);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetFiles(IEnumerable<HttpPostedFileBase> files) 
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                        file.SaveAs(path);
                    }
                }
            }

            return RedirectToAction("Demonstrate", "Service");
        }

        [HttpGet]
        public ActionResult Download(string file = "")
        {
            if (file == "") return null;
            string fullPath = Path.Combine(Server.MapPath("~/App_Data"), file);
            return File(fullPath, "application/zip", file);
        }

        [HttpGet]
        public RedirectToRouteResult Delete(string file = "")
        {
            string fullPath = Request.MapPath("~/Uploads/" + file);
            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch { TempData["Message"] = "You can't delete file " + file; }
            }

            return RedirectToAction("Demonstrate", "Service");
        }
    }
}
