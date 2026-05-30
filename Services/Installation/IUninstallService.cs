namespace LLC_MOD_Toolbox.Services.Installation
{
    public interface IUninstallService
    {
        void DeleteLanguagePack();
        void DeleteBepInEx();
        void DeleteMelonLoader();
        void UninstallAll();
    }
}
