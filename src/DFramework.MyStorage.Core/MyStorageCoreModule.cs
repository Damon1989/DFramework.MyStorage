using System.Reflection;
using Abp.Modules;

namespace DFramework.MyStorage
{
    public class MyStorageCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
