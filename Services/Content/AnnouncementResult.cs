namespace LLC_MOD_Toolbox.Services.Content
{
    public record AnnouncementResult(bool HasNew, string Content, string Level, int Version, Exception? Error = null);
}
