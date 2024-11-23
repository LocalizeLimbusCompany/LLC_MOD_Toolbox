namespace LLC_MOD_Toolbox.Models;

/// <summary>
/// 人格数据
/// </summary>
/// <param name="Name"></param>
/// <param name="Unique">星级（共3个等级）</param>
public record PersonalInfo(string Name, int Unique);
