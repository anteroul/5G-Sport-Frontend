using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartRateBarController : MonoBehaviour
{
    public Image heartRateBarImage; // UI Image for the heart rate bar
    public Sprite[] heartRateSprites; // Sprites for different fullness levels
    public float baseBeatSpeed = 1.0f; // Default speed of beats

    private float currentBPM = 60f; // Default BPM
    private bool isBeating = false;
    private int targetFillIndex = 0; // The fullness level the bar should reach

    void Start()
    {
        StartCoroutine(HeartbeatEffect());
    }

    public void SetHeartRate(float value, float bpm)
    {
        currentBPM = Mathf.Clamp(bpm, 40f, 200f); // Keep BPM within a reasonable range
        targetFillIndex = Mathf.Clamp(Mathf.RoundToInt(value * (heartRateSprites.Length - 1)), 0, heartRateSprites.Length - 1);
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
