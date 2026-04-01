using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string message;
    }

    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private float timeBetweenMessages = 2f;
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float cursorBlinkRate = 0.53f;

    [SerializeField] private float timeBeforeHide = 8f;

    [SerializeField] private GameObject dialogueBox;

    private int currentMessageIndex = 0;
    private float timer = 0f;
    private float cursorTimer = 0f;
    private bool isActive = false;
    private bool isTyping = false;
    private bool allLinesDone = false;
    private bool showCursor = false;

    private string completedText = "";
    private string currentLineText = "";

    private void Start()
    {
        if(dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            if(dialogueBox != null)
            {
                dialogueBox.SetActive(true);
            }

            completedText = "";
            currentLineText = "";
            showCursor = false;
            cursorTimer = 0f;
            isActive = true;
            allLinesDone = false;
            currentMessageIndex = 0;
            timer = 0f;
            ShowNextMessage();
        }
    }

    private void Update()
    {
        if (isActive)
        {
            // Blink cursor until the box hides
            cursorTimer += Time.deltaTime;
            if (cursorTimer >= cursorBlinkRate)
            {
                cursorTimer = 0f;
                showCursor = !showCursor;
                UpdateDisplay();
            }

            if (!isTyping)
            {
                timer += Time.deltaTime;
                if (allLinesDone)
                {
                    if (timer >= timeBeforeHide)
                    {
                        timer = 0f;
                        isActive = false;
                        showCursor = false;
                        UpdateDisplay();
                        if (dialogueBox != null)
                            dialogueBox.SetActive(false);
                    }
                }
                else if (timer >= timeBetweenMessages)
                {
                    timer = 0f;
                    ShowNextMessage();
                }
            }
        }
    }

    private void ShowNextMessage()
    {
        if (currentMessageIndex < dialogueLines.Length)
        {
            StartCoroutine(TypeLine(dialogueLines[currentMessageIndex].message));
            Debug.Log("DIALOGUE: " + dialogueLines[currentMessageIndex].message);
            currentMessageIndex++;
        }
        else
        {
            allLinesDone = true;
            timer = 0f;
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;

        // Add newline separator before this line if previous lines exist
        if (completedText.Length > 0)
            completedText += "\n";

        currentLineText = "";
        showCursor = true;

        foreach (char c in line)
        {
            currentLineText += c;
            showCursor = true;
            UpdateDisplay();
            yield return new WaitForSeconds(typingSpeed);
        }

        completedText += currentLineText;
        currentLineText = "";
        UpdateDisplay();
        isTyping = false;
    }

    private void UpdateDisplay()
    {
        uiText.text = completedText + currentLineText + (showCursor ? "_" : "");
    }

    // Debug, print to console
    public void PrintDialogueToConsole()
    {
        foreach (DialogueLine line in dialogueLines)
        {
            Debug.Log("DIALOGUE: " + line.message);
        }
    }
}
