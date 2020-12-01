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
using ImageMasking.Data;
using Microsoft.Extensions.Configuration;

namespace ImageMasking.Controllers
{
    public class HomeController : Controller
    {
        private int MASK_SIZE = 100;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unit;
        private readonly IConfiguration _configuration;
        
        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger, IConfiguration configuration)
        {
            _unit = unitOfWork;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            MASK_SIZE = configuration.GetValue<int>("MaskSize");
        }

        public IActionResult Index()
        {
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string imagePath = (string)_configuration.GetValue<string>("ImagePath");
            string fullImagePath = (Path.Combine(webRootPath,  imagePath));
            ImageModel image = _unit.ImageRepository.Find(i=>i.Path == fullImagePath).FirstOrDefault();

            if(image == null)
               throw new Exception($"Image not found: {fullImagePath}");
            IEnumerable<PersonOrdersModel> orders = _unit.PersonOrderRepository.Find(o=>o.ImageId == image.Id);
            if(!orders.Any())
                return View(new string[0]);
           
            int[] ids = orders.Select(o=>o.Id).ToArray();
            IEnumerable<PersonModel> personModels = _unit.PersonRepository.Find(p=>ids.Contains(p.Id));

            if(!personModels.Any())
                return View(new string[0]);

            return View(personModels.Distinct().Select(p=>$"{p.FirstName} {p.SecondName}"));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult GetImage()
        {
            string imagePath = (string)_configuration.GetValue<string>("ImagePath");
            DateTime startTime = DateTime.UtcNow;
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string fullImagePath = (Path.Combine(webRootPath,  imagePath));
            ImageProcessing.ImageProcessor processor = new ImageProcessing.ImageProcessor(fullImagePath, MASK_SIZE, _unit);
            
            using(Bitmap image = processor.GetProcessedImage())
            using(MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return File(ms.ToArray(), "image/jpg");
            }
        }
        [HttpGet()]
        public JsonResult GetDonatedPersons()
        {
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string imagePath = (string)_configuration.GetValue<string>("ImagePath");
            string fullImagePath = (Path.Combine(webRootPath,  imagePath));
            ImageModel image = _unit.ImageRepository.Find(i=>i.Path == fullImagePath).FirstOrDefault();

            if(image == null)
               throw new Exception($"Image not found: {fullImagePath}");
            IEnumerable<PersonOrdersModel> orders = _unit.PersonOrderRepository.Find(o=>o.ImageId == image.Id);
            if(!orders.Any())
               return Json(new string[0]);
           
            int[] ids = orders.Select(o=>o.Id).ToArray();
            IEnumerable<PersonModel> personModels = _unit.PersonRepository.Find(p=>ids.Contains(p.Id));

            if(!personModels.Any())
                return Json(new string[0]);

            return Json(personModels.Distinct().Select(p=>$"{p.FirstName} {p.SecondName}" ).ToArray()); 

            
        }
         [HttpPost()]
        public ActionResult Buy([FromForm]string firstName, string secondName, string email)
        {
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string imagePath = (string)_configuration.GetValue<string>("ImagePath");
            string fullImagePath = (Path.Combine(webRootPath,  imagePath));
            var person = _unit.PersonRepository.Find(person=>person.Email == email).FirstOrDefault();
            if(person == null)
            {
                person = new PersonModel(){FirstName = firstName, SecondName = secondName, Email = email};
                
                _unit.PersonRepository.Add(person);
                _unit.Commit();
            }

            ImageModel imageModel = _unit.ImageRepository.Find(image=>image.Path == fullImagePath).FirstOrDefault();
            if(imageModel == null)
                 throw new Exception($"Image not found: {fullImagePath}");

            PersonOrdersModel ordersModel = new PersonOrdersModel(){PersonId = person.Id, ImageId = imageModel.Id, OrderState = 0};
            _unit.PersonOrderRepository.Add(ordersModel);
            _unit.Commit();  

            //PayPal Logic Heree   
         
            ordersModel.OrderState = 1;
            _unit.Commit();

            ImageProcessing.ImageProcessor processor = new ImageProcessing.ImageProcessor(fullImagePath, MASK_SIZE, _unit);
            processor.EditMask(person.Id);
            
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
