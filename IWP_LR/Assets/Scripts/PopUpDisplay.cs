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

    private void Awake()
    {
        // Set the start time
        startTime = Time.time;
    }

    void Start()
    {
        // Get the Text component
        popupText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        // Start the coroutine to handle fading
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            yield return null;

            elapsedTime += Time.deltaTime;

            // Calculate the alpha value based on the elapsed time and fade duration
            float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);

            // Set the alpha value of the text
            Color textColor = popupText.color;
            textColor.a = alpha;
            popupText.color = textColor;
        }

        // Destroy the GameObject when fading is complete
        Destroy(gameObject);
    }
}
