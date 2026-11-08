namespace LLC_MOD_Toolbox.Services.Greytest
{
    public record GreytestValidationResult(bool IsValid, string? Note = null, string? ErrorMessage = null);
}
