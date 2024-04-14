
public class User
{
    public string UserID;
    public string UserName;
    public int Coin;
    public int Level;
    public int Score;
    public int Exp;

    public User(string userID, string userName, int coin, int level, int score, int exp)
    {
        this.UserID = userID;
        this.UserName = userName;
        this.Coin = coin;
        this.Level = level;
        this.Score = score;
        this.Exp = exp;
    }

   
}
