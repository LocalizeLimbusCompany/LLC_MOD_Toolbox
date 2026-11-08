namespace LLC_MOD_Toolbox.Services.Greytest
{
    public interface IGreytestService
    {
        bool IsActive { get; }
        string DownloadUrl { get; }
        Task<GreytestValidationResult> ValidateAndActivateAsync(string token);
    }
}
