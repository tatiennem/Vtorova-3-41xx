using Ninject;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.IO;
using DAL.Interfaces;

namespace Auto
{
    public partial class App : Application
    {
        private IKernel _kernel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Загружаем конфигурацию
          /*  var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
          */

            // Получаем строку подключения
            var connectionString = "Host=localhost;Port=5432;Database=autosalon;Username=postgres;Password=123";

            // Создаем ядро Ninject
            _kernel = new StandardKernel();

            // Загружаем модули в правильном порядке:
            // 1. ServiceModule - для репозиториев (строка подключения)
            _kernel.Load(new BLL.ServiceModule(connectionString));

            // 2. NinjectRegistrations - для сервисов и ViewModels
            _kernel.Load(new NinjectRegistrations());

            // Инициализируем БД (если нужно)
            InitializeDatabase();

            // Создаем MainWindow с ViewModel
            var mainWindow = new MainWindow
            {
                DataContext = _kernel.Get<ViewModels.CatalogViewModel>()
            };

            mainWindow.Show();
        }

        private void InitializeDatabase()
        {
            // Если нужно создать БД или применить миграции
            using var scope = _kernel.BeginBlock();
            var repos = scope.Get<DAL.Interfaces.IDbRepos>();
            // Можно вызвать метод инициализации если есть
        }
    }
}