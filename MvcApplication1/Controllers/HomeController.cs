using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{    
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {            
            string value = System.Web.Configuration.WebConfigurationManager.AppSettings["MaxUploadSize"];

            int MaxUploadSize = 0;
            Int32.TryParse(value, out MaxUploadSize);

            if (MaxUploadSize > 0)
                ViewBag.MaxUploadSize = MaxUploadSize;
            else
                ViewBag.MaxUploadSize = 100; //single file size limit, set 100 KB as default value

            return View();
        }

        /// <summary>
        /// File Upload
        /// </summary>
        /// <returns>string(XMLHttpRequest.responseText)</returns>
        [HttpPost]
        public JsonResult Upload()
        {
            bool isSavedSuccessfully = true;
            string fName = string.Empty;
            string Msg = string.Empty;

            #region check file size
            string value = System.Web.Configuration.WebConfigurationManager.AppSettings["MaxUploadSize"];
            int MaxUploadSize = 0; //KB
            Int32.TryParse(value, out MaxUploadSize);
            MaxUploadSize = (MaxUploadSize == 0) ? 100 : MaxUploadSize; //single file size limit, set 100 KB as default value

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (file.ContentLength > MaxUploadSize * 1024) //ContentLength is in Bytes
                {
                    Msg = "File Size Limit Exceeded " + MaxUploadSize.ToString() + " KB: [" + file.FileName + "]";
                    var json_obj = new
                    {
                        code = -1,
                        msg = Msg
                    };
                    return Json(json_obj);
                }
            }
            #endregion

            try
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    fName = file.FileName;

                    if (file != null && file.ContentLength > 0)
                    {
                        DirectoryInfo originalDirectory = new DirectoryInfo(string.Format("{0}Files", Server.MapPath(@"\")));

                        string pathString = originalDirectory.ToString();
                        bool isExists = System.IO.Directory.Exists(pathString); //Check whether a directory does not exist
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        string path = System.IO.Path.Combine(pathString, fName);
                        if (!System.IO.File.Exists(path))
                        { //file does not exist
                            file.SaveAs(path);
                        }
                        else
                        {
                            isExists = true;
                            int ct = 1;
                            string path_new;
                            while (isExists)
                            {
                                string fn = System.IO.Path.GetFileNameWithoutExtension(path) + "_" + ct.ToString() + System.IO.Path.GetExtension(path);
                                path_new = System.IO.Path.Combine(pathString, fn);
                                isExists = System.IO.File.Exists(path_new);
                                if (!isExists)
                                {
                                    file.SaveAs(path_new);
                                    string s = "File Exists: [" + fName + "], Raname To: [" + fn + "]";
                                    Msg += string.IsNullOrEmpty(Msg) ? s : "|" + s;
                                }
                                ct++;
                            }
                            
                            
                        }
                    }
                    else
                    {
                        isSavedSuccessfully = false;
                        string e = "File Upload Fail: [" + fName + "]";
                        Msg += string.IsNullOrEmpty(Msg) ? e : "|" + e;
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
                Msg = ex.ToString();
            }

            if (isSavedSuccessfully)
            {
                var json_obj = new
                {
                    code = 0,
                    msg = Msg
                };
                return Json(json_obj);
            }
            else
            {
                var json_obj = new
                {
                    code = -1,
                    msg = Msg
                };
                return Json(json_obj);
            }
            
        }

    }
}
