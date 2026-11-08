namespace LLC_MOD_Toolbox.Services.UI
{
    public sealed class DialogInputResult
    {
        public bool IsSuccess { get; init; }
        public bool IsCanceled { get; init; }
        public string? Input { get; init; }
    }
}
