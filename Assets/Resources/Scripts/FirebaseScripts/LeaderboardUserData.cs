[System.Serializable]
public class LeaderboardUserData 
{
    public string Username { get; set; }
    public int ChipCount { get; set; }

    public LeaderboardUserData(string username, int chipCount)
    {
        Username = username;
        ChipCount = chipCount;
    }
}
