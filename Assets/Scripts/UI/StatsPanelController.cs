using UnityEngine;
using TMPro;


public class StatsPanelController : MonoBehaviour
{
    [Header("Player Select")]
    public TMP_Text playerIDText;
    public TMP_Text playerNameText;
    public TMP_Text teamText;

    [Header("Heart Rate Section")]
    public HeartRateBarController heartRateController;

    [Header("Speed Section")]
    public TMP_Text speedText;

    [Header("Endurance Section")]
    public TMP_Text enduranceLevelText;
    public EnduranceBarController enduranceBarController; // Manages visual representation

    public void UpdatePlayerInfo(int playerID, string playerName, string team)
    {
        playerIDText.text = "ID: " + playerID;
        playerNameText.text = playerName;
        teamText.text = team;
    }

    public void UpdateHeartRate(float bpm)
    {
        if (heartRateController != null)
        {
            heartRateController.SetHeartRate(bpm);
        }
    }

    public void UpdateSpeed(float speedKMPH)
    {
        if (speedText != null)
        {
            speedText.text = speedKMPH.ToString("F1");
        }
    }

    public void UpdateEndurance(int level)
    {
        string[] levels = { "BurnOut", "Exhaustion", "Severe Fatigue", "Low", "Mild Fatigue", "Slight Fatigue", "High", "Optimal" };

        if (enduranceLevelText != null)
        {
            enduranceLevelText.text = levels[Mathf.Clamp(level, 0, levels.Length - 1)];
        }

        if (enduranceBarController != null)
        {
            enduranceBarController.SetEnduranceLevel(level);
        }
    }
}
