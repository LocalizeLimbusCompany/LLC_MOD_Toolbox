namespace LLC_MOD_Toolbox.Services
{
    public interface IDialogDisplayService
    {
        public void ShowError(string message);
        public bool Confirm(string message);
    }
}
