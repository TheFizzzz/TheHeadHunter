namespace TheHeadHunter;

internal sealed class ProgressionTier
{
    public ProgressionTier(
        int quality,
        string displayName,
        string bossTrophy,
        string metal,
        string craftingStation,
        int stationLevel)
    {
        Quality = quality;
        DisplayName = displayName;
        BossTrophy = bossTrophy;
        Metal = metal;
        CraftingStation = craftingStation;
        StationLevel = stationLevel;
    }

    public int Quality { get; }
    public string DisplayName { get; }
    public string BossTrophy { get; }
    public string Metal { get; }
    public string CraftingStation { get; }
    public int StationLevel { get; }
}
