using ImageMasking.Models;

namespace ImageMasking.Data
{
    public interface IPersonRepository: IDbRepository<PersonModel>
    {
        
    }

    public class PersonRepository : DbRepository<PersonModel>, IPersonRepository
    {
        public PersonRepository(DataContext dataContext) : base(dataContext)
        {
        }
    }
}