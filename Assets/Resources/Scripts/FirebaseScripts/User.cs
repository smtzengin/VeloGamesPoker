
public class User
{
    private string userID;
    private string userName;
    private int coin;
    private int level;
    private int score;
    private int exp;

    public User(string userID, string userName, int coin, int level, int score, int exp)
    {
        this.userID = userID;
        this.userName = userName;
        this.coin = coin;
        this.level = level;
        this.score = score;
        this.exp = exp;
    }
}
