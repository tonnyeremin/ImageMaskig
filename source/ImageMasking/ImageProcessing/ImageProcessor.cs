using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using ImageMasking.Data;
using ImageMasking.Models;
using Microsoft.Extensions.Logging;

namespace ImageMasking.ImageProcessing
{
    public class ImageProcessor
    {
        private readonly string _fileName;
        private readonly IUnitOfWork _unit;

        public ImageProcessor(string fileName, IUnitOfWork unit)
        {
             _fileName = fileName;
             _unit = unit;
        }
        
        public Bitmap GetProcessedImage()
        {
            Bitmap image =(Bitmap)Image.FromFile( _fileName);
            byte[] mask = GetMask(_fileName);
            ProcessImage(image,mask);
        
            return image;
        }

        public void EditMask(int elementSize, int userId)
        {
            var image = _unit.ImageRepository.Find(i=>i.Path == _fileName).FirstOrDefault();
            if(image == null)
               throw new Exception($"Image not found: {_fileName}");

            var mask = _unit.MaskRepository.Find(m=>m.ImageId == image.Id).FirstOrDefault();
            if(mask == null)
                throw new Exception($"Mask for image not found: {_fileName}");


            _unit.MaskRepository.Update(mask);
            _unit.Commit();      
        }

        private byte[] GetMask(string filePath)
        {
           if(IsMaskExists(filePath))
           {
               return LoadExistingMask(filePath);
           }
           else
           {
               return CreateNewMask(filePath);
           }
        }

        private bool IsMaskExists(string filePath)
        {
            var image = _unit.ImageRepository.Find(i=>i.Path == filePath).FirstOrDefault();
            if(image == null)
                return false;
            var mask = _unit.MaskRepository.Find(m=>m.ImageId == image.Id).FirstOrDefault();

            return mask != null;
        }

        private byte[] CreateNewMask(string filePath)
        {
            var imageModel = _unit.ImageRepository.Find(i=>i.Path == filePath).FirstOrDefault();
            if(imageModel  == null)
            {
                imageModel  = new ImageModel(){Path = filePath};
                _unit.ImageRepository.Add(imageModel);
            }

            using(Bitmap image =(Bitmap)Image.FromFile(filePath))
            {
                byte[] mask = new byte[image.Width*image.Height];
                for(int i =0; i<mask.Length; i++)
                    mask[i] =  0;

                MaskModel maskModel = new MaskModel(){ImageId = imageModel.Id, MaskArray = mask, MaskHeight = image.Height, MaskWidth = image.Width};
                _unit.MaskRepository.Add(maskModel);
                _unit.Commit();    

                return mask;
            }
        }

        private byte[] LoadExistingMask(string filePath)
        {
            var image = _unit.ImageRepository.Find(i=>i.Path == filePath).FirstOrDefault();
            if(image == null)
               throw new Exception($"Image not found: {filePath}");

            var mask = _unit.MaskRepository.Find(m=>m.ImageId == image.Id).FirstOrDefault();
            if(mask == null)
                throw new Exception($"Mask for image not found: {filePath}");

            byte[] maskArray = new byte[mask.MaskHeight*mask.MaskWidth];
            for(int i =0 ; i< maskArray.Length; i++)
                maskArray[i] =(byte)(mask.MaskArray[i]>0? 1:0);

            return maskArray;
        }

        private unsafe void ProcessImage(Bitmap image, byte[] mask)
        {
            DateTime startTime = DateTime.UtcNow;
            var bitmapData = image.LockBits (
                                new Rectangle (0, 0, image.Width, image.Height),
                                ImageLockMode.ReadWrite, 
                                image.PixelFormat
                            );

            int pixelSize =3;
               
            int nWidth = bitmapData.Width * pixelSize;
            int nHeight = bitmapData.Height;

            for(int pane = 0; pane<pixelSize; pane++)
            {
                int maskRow = 0;
                for(int y = 0; y<nHeight; y++)
                {
                    byte*  currentRow =(byte*)(void*)bitmapData.Scan0 + y*bitmapData.Stride;  
                    int maskColumn = maskRow*image.Width;
                    for (int x = 0; x < bitmapData.Width; x++)
                    {
                            currentRow[pane] = mask[maskColumn]!=0 ?  currentRow[pane] : (byte)255;
                            currentRow+=pixelSize;
                            maskColumn++;
                    }

                    maskRow++;
                    }
                  
                }

            image.UnlockBits(bitmapData);
        }
    }
}