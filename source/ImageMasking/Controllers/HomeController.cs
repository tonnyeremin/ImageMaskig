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

        public unsafe ActionResult GetImage()
        {
            DateTime startTime = DateTime.UtcNow;
            string webRootPath = _hostingEnvironment.ContentRootPath;
            
            using(Bitmap image =(Bitmap)Image.FromFile(Path.Combine(webRootPath, "Images\\test.jpg")))
            using(MemoryStream ms = new MemoryStream())
            {
                int maskSize = 3;
                int maskWidth =  (int)image.Width/maskSize ;
                if(maskWidth%3>0)
                    maskSize = (int)(image.Width-1)/maskSize ;

                int maskHeight =  (int)image.Height/maskSize ;
                if(maskHeight%3>0)
                    maskHeight = (int)(image.Height-1)/maskSize ;

                byte[] mask = new byte[maskHeight*maskWidth];
                for(int i=0; i< mask.Length; i++)
                {
                    if(i%5 ==0)
                     mask[i] =  1; 
                     else
                      mask[i] =  0; 
                }    

                var bitmapData = image.LockBits (
                    new Rectangle (0, 0, image.Width, image.Height),
                    ImageLockMode.ReadWrite, 
                    image.PixelFormat
                );

                int pixelSize =3;
                byte* current =(byte*)(void*)bitmapData.Scan0;
                int nWidth =bitmapData.Width * pixelSize;
                int nHeight = bitmapData.Height;

                for (int y =0; y < maskHeight; y++)
                {
                    current = (byte*)(void*)bitmapData.Scan0 + image.Width*y*maskSize* pixelSize;
                    for (int x =0; x <maskWidth;x++ )
                    {
                        if (x % pixelSize ==0|| x ==0)
                        {
                            if(mask[x*y]>0)
                            {
                                for(int i =0; i< maskSize*maskSize; i++)
                                {
                                     current[i] = 255;
                                }
                            }
                        }
                        current+=maskSize*pixelSize;
                    }
                }
                
                image.UnlockBits(bitmapData);
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                _logger.LogInformation("Image processing time: {0} ms", (DateTime.UtcNow - startTime).TotalMilliseconds);
                return File(ms.ToArray(), "image/jpg");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
