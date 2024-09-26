using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PDF_Viewer.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace PDF_Viewer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _environment = environment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<string> list = new List<string>();


            string uploadedPath = Path.Combine(_environment.WebRootPath + "/Uploads");
            
            if (!Directory.Exists(uploadedPath))
            {
                Directory.CreateDirectory(uploadedPath);
            }

            string[] filePaths = Directory.GetFiles(uploadedPath);

            foreach(var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);
                list.Add(fileName);
               
            }
            return View(list);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadPdf(IFormFile Pdf)
        {

            if (Pdf != null)
            {
                string fileName = Pdf.FileName;
                string uploadPath = Path.Combine(_environment.WebRootPath+"/Uploads", fileName);
                using (FileStream stream = new FileStream(uploadPath, FileMode.Create)) { 
                    Pdf.CopyTo(stream);
                }
            }
            return RedirectToAction("Index");
        }

       
        [HttpPost]
        public IActionResult PDFView(string fileName)
        {
            /*if (string.IsNullOrEmpty(fileName))
                return ("File not uploded");*/

            string fullFilePath = string.Empty;
            string currentDirectory = Path.Combine(_environment.WebRootPath, "Uploads");

            string[] fullFilePathArray = Directory.GetFiles(currentDirectory, fileName, SearchOption.AllDirectories);
            fullFilePath = (fullFilePathArray.Length > 0) ? string.Join("", fullFilePathArray[0]) : string.Empty;

            if (!System.IO.File.Exists(fullFilePath))
                return new JsonResult(new { success = false, message = "File not found" });

            string viewFileContent = fullFilePath.Substring(_environment.WebRootPath.Length + 1);
            string virtualPath = Url.Content(viewFileContent);
            string searializedString = JsonConvert.SerializeObject(virtualPath);

            return new JsonResult(new { success = true, Data = searializedString });


        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
