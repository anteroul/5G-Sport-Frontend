using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnduranceBarController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image enduranceBarImage;              // UI Image showing endurance level
    public Sprite[] enduranceSprites;            // 8 Sprites from BurnOut (0) to Optimal (7)
    public TMP_Text enduranceLabelText;          // Optional: Text displaying the endurance label

    private readonly string[] levelLabels = {
        "BurnOut", "Exhaustion", "Severe Fatigue", "Low",
        "Mild Fatigue", "Slight Fatigue", "High", "Optimal"
    };

    /// <summary>
    /// Sets the endurance level visually and optionally via label.
    /// </summary>
    /// <param name="level">Endurance level: 0 = BurnOut, 7 = Optimal</param>
    public void SetEnduranceLevel(int level)
    {
        int clampedLevel = Mathf.Clamp(level, 0, 7);

        // Update sprite
        if (enduranceBarImage != null && enduranceSprites != null && enduranceSprites.Length > clampedLevel)
        {
            enduranceBarImage.sprite = enduranceSprites[clampedLevel];
        }

        // Update label
        if (enduranceLabelText != null)
        {
            enduranceLabelText.text = levelLabels[clampedLevel];
        }
    }
}
