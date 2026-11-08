namespace LLC_MOD_Toolbox.Services.Font
{
    public interface IFontService
    {
        bool IsValidFontFile(string filePath);
        string GetFontFamilyName(string filePath);
        void ApplyFontReplacement(string fontFilePath, string limbusDir);
        void RestoreOriginalFont(string limbusDir);
    }
}
