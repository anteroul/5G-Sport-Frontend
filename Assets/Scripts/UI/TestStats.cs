using UnityEngine;
using TMPro;

public class TestStats : MonoBehaviour
{

    public StatsPanelController statsPanel;

    [Header("Player Info Test")]
    public int testPlayerID = 1;
    public string testPlayerName = "Test Player";
    public string testTeam = "Red";

    [Header("Heart Rate Test")]
    [Range(40, 200)] public float testBPM = 80f;

    [Header("Speed Test")]
    [Range(0, 40)] public float testSpeed = 10f;

    [Header("Endurance Test")]
    [Range(0, 7)] public int testEnduranceLevel = 5;

    void Start()
    {
        if (statsPanel != null)
        {
            statsPanel.UpdatePlayerInfo(testPlayerID, testPlayerName, testTeam);
            statsPanel.UpdateHeartRate(testBPM);
            statsPanel.UpdateSpeed(testSpeed);
            statsPanel.UpdateEndurance(testEnduranceLevel);
        }
    }

    void Update()
    {
        if (statsPanel != null)
        {
            statsPanel.UpdateHeartRate(testBPM);
            statsPanel.UpdateSpeed(testSpeed);
            statsPanel.UpdateEndurance(testEnduranceLevel);
        }
    }


}
