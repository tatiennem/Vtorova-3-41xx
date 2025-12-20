using Auto;
using Auto.Services;
using Auto.Services.Interfaces;
using Auto.ViewModels;
using Domain;
using DAL.DbContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Auto
{
    public partial class App : Application
    {
        private ServiceProvider? _services;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);

            _services = serviceCollection.BuildServiceProvider();

            using (var scope = _services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
                await initializer.InitializeAsync();
            }

            var mainWindow = _services.GetRequiredService<MainWindow>();
            var mainVm = _services.GetRequiredService<MainViewModel>();
            mainWindow.DataContext = mainVm;
            if (mainVm.CurrentViewModel is IRefreshable refreshable)
            {
                await refreshable.RefreshAsync(force: true);
            }
            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
            services.Configure<ContractOptions>(configuration.GetSection("Contracts"));

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddDbContextFactory<AutoSalonContext>((sp, options) => DatabaseProviderResolver.Configure(sp, options),
                ServiceLifetime.Transient);

            services.AddScoped<DbInitializer>();
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<ISalesService, SalesService>();
            services.AddScoped<ITestDriveService, TestDriveService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IContractService, ContractService>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<CatalogViewModel>();
            services.AddTransient<TestDriveViewModel>();
            services.AddTransient<ReportsViewModel>();

            services.AddSingleton<MainWindow>();
        }
    }
}
