using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Extensions.Logging;

namespace ImageMasking.ImageProcessing
{
    public class ImageProcessor
    {
        private string _fileName;

        public ImageProcessor(string fileName)
        {
             _fileName = fileName;
        }
        
        public  Bitmap GetProcessedImage()
        {
            Bitmap image =(Bitmap)Image.FromFile( _fileName);
            byte[] mask = GetMask(_fileName);
            ProcessImage(image,mask);
        
            return image;
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
            return false;
        }

        private byte[] CreateNewMask(string filePath)
        {
            using(Bitmap image =(Bitmap)Image.FromFile(filePath))
            {
                byte[] mask = new byte[image.Width*image.Height];
                for(int i =0; i<mask.Length; i++)
                    mask[i] = 1;
                return  mask;
            }
        }

        private byte[] LoadExistingMask(string filePath)
        {
             return new byte[0];
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
                            currentRow[pane] = mask[maskColumn]!=0 ? (byte)255 : currentRow[pane];
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