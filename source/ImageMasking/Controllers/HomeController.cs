using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImageMasking.Models;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace ImageMasking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        
        public HomeController(IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult GetImage()
        {
            DateTime startTime = DateTime.UtcNow;
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string imagePath = (Path.Combine(webRootPath, "Images\\test.jpg"));
            ImageProcessing.ImageProcessor processor = new ImageProcessing.ImageProcessor(imagePath);
            
            using(Bitmap image = processor.GetProcessedImage())
            using(MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return File(ms.ToArray(), "image/jpg");
            }
        }
         [HttpPost()]
        public ActionResult Buy([FromForm]string firstName, string secondName, string email)
        {
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
