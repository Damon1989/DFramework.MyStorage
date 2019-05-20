using System.Data.Entity;
using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using DFramework.MyStorage.EntityFramework;

namespace DFramework.MyStorage
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(MyStorageCoreModule))]
    public class MyStorageDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "StorageDbContextConnectionString";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Database.SetInitializer<MyStorageDbContext>(null);
        }
    }
}
