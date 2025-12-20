using System.Threading.Tasks;
using System.Windows;
using Auto.Services.Interfaces;

namespace Auto.Services
{
    public class NotificationService : INotificationService
    {
        public Task ShowInfoAsync(string message, string title = "Информация")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public Task ShowErrorAsync(string message, string title = "Ошибка")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            return Task.CompletedTask;
        }
    }
}
