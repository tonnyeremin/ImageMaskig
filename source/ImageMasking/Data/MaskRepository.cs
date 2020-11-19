using ImageMasking.Models;

namespace ImageMasking.Data
{
    public interface IMaskRepository : IDbRepository<MaskModel>
    {
        
    }

    public class MaskReposiotry : DbRepository<MaskModel>, IMaskRepository
    {
        public MaskReposiotry(DataContext dataContext) : base(dataContext)
        {
        }
    }
}