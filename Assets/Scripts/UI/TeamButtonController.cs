using UnityEngine;
using UnityEngine.UI;

public class TeamButtonController : MonoBehaviour
{
    public GameObject team1Viewport; // Assign PlayerScrollView_Team1 in Inspector
    public GameObject team2Viewport; // Assign PlayerScrollView_Team2 in Inspector

    public Button team1Button; // Assign Team1Button in Inspector
    public Button team2Button; // Assign Team2Button in Inspector

    void Start()
    {
        // Ensure Team 1 is visible, Team 2 is hidden at start
        ShowTeam1();

        // Assign button listeners
        team1Button.onClick.AddListener(ShowTeam1);
        team2Button.onClick.AddListener(ShowTeam2);
    }

    void ShowTeam1()
    {
        team1Viewport.SetActive(true);
        team2Viewport.SetActive(false);
    }

    void ShowTeam2()
    {
        team1Viewport.SetActive(false);
        team2Viewport.SetActive(true);
    }
}
