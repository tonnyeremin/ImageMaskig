namespace ImageMasking.Data
{
    public interface IUnitOfWork
    {

        IImageRepository ImageRepository{get;}
        IMaskRepository MaskRepository{get;}
        IPersonRepository PersonRepository{get;}

        void Commit();

    }
    public class UnitOfWorkImp : IUnitOfWork
    {
        
        private readonly DataContext _dataContext;
        public  UnitOfWorkImp(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        private IImageRepository _imageRepository;
        public IImageRepository ImageRepository
        {
            get
            {
                if(_imageRepository == null)
                    _imageRepository = new ImageRepository(_dataContext);

                return _imageRepository;    
            }
        }

        private IMaskRepository _maskRepository;
        public IMaskRepository MaskRepository
        {
            get
            {
                if(_maskRepository == null)
                    _maskRepository = new MaskReposiotry(_dataContext);

                return _maskRepository;    
            }
        }
        private IPersonRepository _personRepository;
        public IPersonRepository PersonRepository
        {
            get
            {
                if(_personRepository == null)
                    _personRepository = new PersonRepository(_dataContext);

                return _personRepository;
            }
        }

        public void Commit()
        {
            _dataContext.SaveChanges();
        }
    }
}