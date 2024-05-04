
public class User
{
    public string UserID;
    public string Username;
    public int Chip;
    public int Level;
    public int Score;
    public int Exp;

    public User(string userID, string userName, int chip, int level, int score, int exp)
    {
        this.UserID = userID;
        this.Username = userName;
        this.Chip = chip;
        this.Level = level;
        this.Score = score;
        this.Exp = exp;
    }

   
}
