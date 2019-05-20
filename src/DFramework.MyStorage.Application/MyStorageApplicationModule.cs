using System.Reflection;
using Abp.Modules;

namespace DFramework.MyStorage
{
    [DependsOn(typeof(MyStorageCoreModule))]
    public class MyStorageApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
