using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpDisplay : MonoBehaviour
{
    public float fadeDuration = 2f;
    public float displayDuration = 5f;

    private TMP_Text popupText;
    private float startTime;

    void Start()
    {
        // Get the Text component
        popupText = GetComponent<TMP_Text>();

        // Set the start time
        startTime = Time.time;

        // Start the coroutine to handle fading
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Calculate the elapsed time since the start
        float elapsedTime = Time.time - startTime;

        // Calculate the alpha value based on the elapsed time and fade duration
        float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);

        // Set the alpha value of the text
        Color textColor = popupText.color;
        textColor.a = alpha;
        popupText.color = textColor;

        // If the text is still visible, continue fading
        if (alpha > 0f)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            // Destroy the GameObject when fading is complete
            Destroy(gameObject);
        }
    }
}
