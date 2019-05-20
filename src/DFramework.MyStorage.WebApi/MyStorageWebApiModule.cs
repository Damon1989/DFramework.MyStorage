using System.Reflection;
using Abp.Application.Services;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.WebApi;

namespace DFramework.MyStorage
{
    [DependsOn(typeof(AbpWebApiModule), typeof(MyStorageApplicationModule))]
    public class MyStorageWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(MyStorageApplicationModule).Assembly, "app")
                .Build();
        }
    }
}
