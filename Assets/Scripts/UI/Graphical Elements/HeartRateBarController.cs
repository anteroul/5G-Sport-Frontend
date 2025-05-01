using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HeartRateBarController : MonoBehaviour
{
    public Image heartRateBarImage; // UI Image for the heart rate bar
    public Sprite[] heartRateSprites; // Sprites for different fullness levels
    public TMP_Text bpmText; // UI Text for BPM
    public TMP_Text statusText; // UI Text for Heart Rate status

    private float currentBPM = 60f; // Default BPM
    private bool isBeating = false;
    private int targetFillIndex = 0; // The fullness level the bar should reach

    void Start()
    {
        StartCoroutine(HeartbeatEffect());
    }

    public void SetHeartRate(float bpm)
    {
        currentBPM = Mathf.Clamp(bpm, 40f, 200f); // Keep BPM within a reasonable range
        targetFillIndex = GetFillIndex(currentBPM); // Get sprite level
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update BPM Text
        if (bpmText != null)
        {
            bpmText.text = currentBPM.ToString("F0");
        }

        // Update Status Text
        if (statusText != null)
        {
            statusText.text = GetHeartRateStatus(currentBPM);
        }

        // Update Sprite
        if (heartRateBarImage != null)
        {
            heartRateBarImage.sprite = heartRateSprites[targetFillIndex];
        }
    }

    private string GetHeartRateStatus(float bpm)
    {
        if (bpm < 60) return "Resting";
        else if (bpm < 80) return "Very Light";
        else if (bpm < 100) return "Light";
        else if (bpm < 120) return "Vigorous";
        else if (bpm < 140) return "High";
        else return "Very High";
    }

    private int GetFillIndex(float bpm)
    {
        if (bpm < 60) return 0;
        else if (bpm < 80) return 1;
        else if (bpm < 100) return 2;
        else if (bpm < 120) return 3;
        else if (bpm < 140) return 4;
        else return 5;
    }

    private IEnumerator HeartbeatEffect()
    {
        while (true)
        {
            if (isBeating) yield return null;

            isBeating = true;

            float beatInterval = 60f / currentBPM; // Convert BPM to time in seconds
            float fillTime = beatInterval * 0.3f;  // 30% of the beat time for filling
            float holdTime = beatInterval * 0.2f;  // 20% of the beat time to stay full
            float drainTime = beatInterval * 0.5f; // 50% of the beat time for draining

            // Smoothly fill up
            yield return StartCoroutine(SmoothTransition(0, targetFillIndex, fillTime));

            // Hold at full for a moment
            yield return new WaitForSeconds(holdTime);

            // Smoothly drain back down
            yield return StartCoroutine(SmoothTransition(targetFillIndex, 0, drainTime));

            isBeating = false;
        }
    }

    private IEnumerator SmoothTransition(int startIndex, int endIndex, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int spriteIndex = Mathf.RoundToInt(Mathf.Lerp(startIndex, endIndex, t));

            heartRateBarImage.sprite = heartRateSprites[spriteIndex];

            yield return null;
        }
    }
}
