namespace LLC_MOD_Toolbox.Services.Skin
{
    public interface ISkinMusicService
    {
        void PlayWithFadeIn(string musicPath);
        void FadeOutAndStop();
        Task FadeOutAndStopAsync();
        void SetVolumeMultiplier(double multiplier);
    }
}
