namespace LLC_MOD_Toolbox.Services
{
    public interface IDialogDisplayService
    {
        void ShowError(string message);
        bool Confirm(string message);
    }
}
