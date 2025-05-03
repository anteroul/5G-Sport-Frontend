using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedBarController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image speedBarImage;           // UI Image showing speed level
    public Sprite[] speedSprites;         // Sprites representing different speed levels
    public TMP_Text speedLabelText;       // Optional: Text displaying the speed label

    /// <summary>
    /// Sets the speed level visually and optionally via label.
    /// </summary>
    /// <param name="speedKMPH">Speed in kilometers per hour.</param>
    public void SetSpeedLevel(float speedKMPH)
    {
        int level = 0; // Default to sprite 0 (empty bar)

        if (speedKMPH >= 1f && speedKMPH < 5f)
        {
            level = 1;
        }
        else if (speedKMPH >= 5f && speedKMPH < 10f)
        {
            level = 2;
        }
        else if (speedKMPH >= 10f && speedKMPH < 15f)
        {
            level = 3;
        }
        else if (speedKMPH >= 15f)
        {
            level = 4;
        }

        // Update sprite
        if (speedBarImage != null && speedSprites != null && speedSprites.Length > level)
        {
            speedBarImage.sprite = speedSprites[level];
        }

        /*
        // Optional: Update label
        if (speedLabelText != null)
        {
            speedLabelText.text = $"Speed: {speedKMPH:F1} km/h";
        }
        */
    }
}
