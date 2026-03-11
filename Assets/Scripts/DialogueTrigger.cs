using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    // set string in Inspector
    //[SerializeField] private string dialogueMessage = "Testing Testing I'm Just Suggesting";
    [SerializeField] private string[] dialogueMessages;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private float timeBetweenMessages = 2f;

    private int currentMessageIndex = 0;
    private float timer = 0f;
    private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            uiText.text = "";
            isActive = true;
            currentMessageIndex = 0;
            timer = 0f;
            ShowNextMessage();
        }
    }

    private void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenMessages)
            {
                timer = 0f;
                ShowNextMessage();
            }
        }
    }

    private void ShowNextMessage()
    {
        if (currentMessageIndex < dialogueMessages.Length)
        {
            uiText.text += ">" + dialogueMessages[currentMessageIndex] + "\n";
            Debug.Log("DIALOGUE: " + dialogueMessages[currentMessageIndex]);
            currentMessageIndex++;
        }
        else
        {
            // uiText.text = "";
            isActive = false;
        }
    }

    // Debug, print to console
    public void PrintDialogueToConsole()
    {
        foreach (string message in dialogueMessages)
        {
            Debug.Log("DIALOGUE: " + message);
        }
    }
}