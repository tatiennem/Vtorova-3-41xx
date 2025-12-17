using DAL;
using DAL.Interfaces;
using Ninject.Modules;

namespace BLL
{
    public class ServiceModule : NinjectModule
    {
        private string _connectionString;

        public ServiceModule(string connection)
        {
            _connectionString = connection;
        }

        public override void Load()
        {
            // Регистрируем репозиторий с передачей строки подключения
                Bind<IDbRepos>()
                .To<DbReposSQL>()
                .InSingletonScope()
                .WithConstructorArgument("connectionString", _connectionString);
        }
    }
}