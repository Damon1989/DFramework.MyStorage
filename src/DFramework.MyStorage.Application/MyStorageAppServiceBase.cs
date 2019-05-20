using Abp.Application.Services;

namespace DFramework.MyStorage
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class MyStorageAppServiceBase : ApplicationService
    {
        protected MyStorageAppServiceBase()
        {
            LocalizationSourceName = MyStorageConsts.LocalizationSourceName;
        }
    }
}