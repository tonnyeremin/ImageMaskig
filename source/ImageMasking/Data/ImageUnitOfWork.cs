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
        
        public IImageRepository ImageRepository => throw new System.NotImplementedException();

        public IMaskRepository MaskRepository => throw new System.NotImplementedException();

        public IPersonRepository PersonRepository => throw new System.NotImplementedException();

        public void Commit()
        {
            throw new System.NotImplementedException();
        }
    }
}