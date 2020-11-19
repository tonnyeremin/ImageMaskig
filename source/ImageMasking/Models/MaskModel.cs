using System;

namespace ImageMasking.Models
{
    public class MaskModel
    {
        public int Id{get; set;}
        public int ImageId{get; set;}
        public int MaskHeight{get; set;}
        public int MaskWidth{get; set;}
        public int[] MaskArray{get; set;}
    }
}