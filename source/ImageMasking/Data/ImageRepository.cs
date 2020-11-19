using ImageMasking.Models;

namespace ImageMasking.Data
{
    public interface IImageRepository : IDbRepository<ImageModel>
    {
        
    }

    public class ImageRepository : DbRepository<ImageModel>, IImageRepository
    {
        public ImageRepository(DataContext context) : base(context)
        {
        }
    }

    
}