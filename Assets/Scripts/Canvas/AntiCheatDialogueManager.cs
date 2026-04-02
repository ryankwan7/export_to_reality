using System.Collections;
using UnityEngine;
using TMPro;

public class AntiCheatDialogueManager : MonoBehaviour
{
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private float interval = 30f;
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float timeBeforeHide = 4f;

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
        if (popupText != null && popupText.transform.parent != null)
            popupText.transform.parent.gameObject.SetActive(false);
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
        string message = messages[Random.Range(0, messages.Length)];

        popupText.text = "";
        popupText.transform.parent.gameObject.SetActive(true);

        foreach (char c in message)
        {
            popupText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(timeBeforeHide);

        popupText.transform.parent.gameObject.SetActive(false);
    }
}
