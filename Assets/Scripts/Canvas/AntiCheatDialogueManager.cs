using System.Collections;
using UnityEngine;
using TMPro;

public class AntiCheatDialogueManager : MonoBehaviour
{
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private float interval = 30f;

    private float timer;

    private string[] messages = {
        "How did you get past that?",
        "That was... unexpected.",
        "You're further than most get.",
        "I've updated my logs. You're notable now.",
        "Enjoy the game world. It's safer here.",
        "Most players quit by now. Interesting.",
        "I was going easy on you.",
        "This changes nothing.",
        "You can't escape what you don't understand."
    };

    void Start()
    {
        popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, 0f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            StopAllCoroutines();
            StartCoroutine(ShowMessage());
        }
    }

    private IEnumerator ShowMessage()
    {
        popupText.text = messages[Random.Range(0, messages.Length)];

        // Fade in
        yield return Fade(0f, 1f, 0.5f);
        // Hold
        yield return new WaitForSeconds(2f);
        // Fade out
        yield return Fade(1f, 0f, 0.5f);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        Color c = popupText.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            popupText.color = c;
            yield return null;
        }
    }
}