using ImageMasking.Models;

namespace ImageMasking.Data
{
    public interface IPersonOrderRepository: IDbRepository<PersonOrdersModel>
    {
        
    }
    public class PersonOrderRepository : DbRepository<PersonOrdersModel>, IPersonOrderRepository
    {
        public PersonOrderRepository(DataContext dataContext) : base(dataContext)
        {
        }
    }
}