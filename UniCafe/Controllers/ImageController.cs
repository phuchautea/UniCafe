using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniCafe.Controllers
{
    public class ImageController : Controller
    {
        [HttpPost]
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }

            var fileName = file.FileName;
            var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
            if (System.IO.File.Exists(path))
            {
                var extension = Path.GetExtension(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString() + extension;
            }

            file.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), fileName));
            return "/Content/images/" + fileName;
        }
    }
}