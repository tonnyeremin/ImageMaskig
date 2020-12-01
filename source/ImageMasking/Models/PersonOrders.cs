namespace ImageMasking.Models
{
    public class PersonOrdersModel
    {
       public int Id{get; set;}
       public int PersonId{get; set;}
       public int ImageId{get; set;}

       public int OrderState{get; set;}
    }
}