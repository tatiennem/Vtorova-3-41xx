namespace Auto.ViewModels
{
    public interface IRefreshable
    {
        Task RefreshAsync(bool force = false);
    }
}
