namespace LLC_MOD_Toolbox.Services.Content
{
    public interface ILoadingTextService
    {
        Task InitializeAsync();
        Task RefreshAsync();
        string GetRandomLoadingText();
    }
}
