public class Team
{
    public enum TeamID
    {
        ONE = 0,
        TWO
    };

    public bool ID;
    public string TeamName;

    Team(bool iD, string teamName)
    {
        ID = iD;
        TeamName = teamName;
    }
}