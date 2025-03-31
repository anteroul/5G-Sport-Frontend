using UnityEngine;
using TMPro; // Required for TextMeshPro

public class StatsPanel : MonoBehaviour
{
    public TMP_Text playerIDText;
    public TMP_Text playerNameText;
    public TMP_Text teamText;

    public void UpdateStatsPanel(int playerID, string playerName)
    {
        playerIDText.text = "ID: " + playerID;
        playerNameText.text = playerName;
        teamText.text = playerID <= 11 ? "RED" : "BLUE";
    }
}