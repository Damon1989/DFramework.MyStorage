using Abp.Web.Mvc.Controllers;

namespace DFramework.MyStorage.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class MyStorageControllerBase : AbpController
    {
        protected MyStorageControllerBase()
        {
            LocalizationSourceName = MyStorageConsts.LocalizationSourceName;
        }
    }
}