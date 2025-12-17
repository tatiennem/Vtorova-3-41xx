using Ninject.Modules;
using Interfaces.Services;
using Infrastructure.Services;
using Auto.ViewModels;

namespace Auto
{
    public class NinjectRegistrations : NinjectModule
    {
        public override void Load()
        {
            // Регистрируем сервисы
            Bind<ICatalogService>().To<CatalogService>();

            // Регистрируем ViewModels
            Bind<CatalogViewModel>().ToSelf();

            
        }
    }
}