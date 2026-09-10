namespace TheHeadHunter;

internal sealed class ProgressionTier
{
    public ProgressionTier(
        int quality,
        string displayName,
        string bossTrophy,
        string metal,
        int forgeLevel)
    {
        Quality = quality;
        DisplayName = displayName;
        BossTrophy = bossTrophy;
        Metal = metal;
        ForgeLevel = forgeLevel;
    }

    public int Quality { get; }
    public string DisplayName { get; }
    public string BossTrophy { get; }
    public string Metal { get; }
    public int ForgeLevel { get; }
}
