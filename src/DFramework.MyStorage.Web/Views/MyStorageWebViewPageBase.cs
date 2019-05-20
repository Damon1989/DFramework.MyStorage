using Abp.Web.Mvc.Views;

namespace DFramework.MyStorage.Web.Views
{
    public abstract class MyStorageWebViewPageBase : MyStorageWebViewPageBase<dynamic>
    {

    }

    public abstract class MyStorageWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected MyStorageWebViewPageBase()
        {
            LocalizationSourceName = MyStorageConsts.LocalizationSourceName;
        }
    }
}