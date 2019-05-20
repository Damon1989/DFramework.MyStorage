using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace DFramework.MyStorage.EntityFramework.Repositories
{
    public abstract class MyStorageRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<MyStorageDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected MyStorageRepositoryBase(IDbContextProvider<MyStorageDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class MyStorageRepositoryBase<TEntity> : MyStorageRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected MyStorageRepositoryBase(IDbContextProvider<MyStorageDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
