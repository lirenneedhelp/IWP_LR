using UnityEngine;
using TMPro;
public class TextColourBlink : MonoBehaviour
{
    TMP_Text text;

    public Color startColour;

    public Color endColour;

    [Range(0, 10)]
    public float speed;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }
    // Update is called once per frame
    void Update()
    {
        text.color = Color.Lerp(startColour, endColour, Mathf.PingPong(Time.time * speed, 1));
    }
}
