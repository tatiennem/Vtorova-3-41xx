using System.Threading.Tasks;

namespace Auto.Services.Interfaces
{
    public interface INotificationService
    {
        Task ShowInfoAsync(string message, string title = "Информация");
        Task ShowErrorAsync(string message, string title = "Ошибка");
    }
}
