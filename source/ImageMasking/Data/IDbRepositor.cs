using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ImageMasking.Data
{
    public interface IDbRepository<T> where T : class
    {
         IEnumerable<T> GetAll();
         T GetByIt(int id);
         IEnumerable<T> Find(Expression<Func<T, bool>> expression);     
         void Add(T entity);
         void Update(T newEntity);
         void Remove(T entity);

    }
}