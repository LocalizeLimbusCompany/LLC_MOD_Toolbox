using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.Services.Gacha
{
    public record GachaRollResult(List<PersonalInfo> Personals, int[] UniqueCounts, bool HasVergil, int RollCount);
}
