using Auto;
using Auto.Services;
using Auto.Services.Interfaces;
using Auto.ViewModels;
using AutoSalonToyota.Services;
using DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;
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

            

            var mainWindow = _services.GetRequiredService<MainWindow>();
            var mainVm = _services.GetRequiredService<MainViewModel>();
            mainWindow.DataContext = mainVm;
           
            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddDbContextFactory<AutoSalonContext>(options =>
            {
                var provider = configuration["Database:Provider"];
                var connectionString = configuration["Database:ConnectionString"];

                if (provider == "Postgres" && !string.IsNullOrEmpty(connectionString))
                {
                    // Для PostgreSQL
                    options.UseNpgsql(connectionString);
                }
                
            });

            services.Configure<ContractOptions>(configuration.GetSection("Contracts"));

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<INotificationService, NotificationService>();
           

           
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<ISalesService, SalesService>();
            
            services.AddScoped<IContractService, ContractService>();

            services.AddSingleton<MainViewModel>();
            services.AddTransient<CatalogViewModel>();
            services.AddScoped<ITestDriveService, TestDriveService>();
            services.AddTransient<TestDriveViewModel>();


            services.AddSingleton<MainWindow>();
        }
    }
}
