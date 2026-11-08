namespace LLC_MOD_Toolbox.Services.Content
{
    public interface IAnnouncementService
    {
        Task<AnnouncementResult> CheckForAnnouncementAsync();
        void MarkAsRead(int version);
    }
}
