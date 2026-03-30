using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    // set string in Inspector
    //[SerializeField] private string dialogueMessage = "Testing Testing I'm Just Suggesting";

    [System.Serializable]
    public struct DialogueLine
    {
        public string message;
        public Color color;
    }

    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private float timeBetweenMessages = 2f;

    [SerializeField] private float timeBeforeHide = 8f;

    [SerializeField] private GameObject dialogueBox;

    private int currentMessageIndex = 0;
    private float timer = 0f;
    private bool isActive = false;

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
        else
        {
            timer += Time.deltaTime;
            if (timer >= timeBeforeHide)
            {
                timer = 0f;
                if(dialogueBox != null){
                    dialogueBox.SetActive(false);  
                }
            }
        }
    }

    private void ShowNextMessage()
    {
        if (currentMessageIndex < dialogueLines.Length)
        {
            DialogueLine line = dialogueLines[currentMessageIndex];
            
            // Apply color using rich text
            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(line.color)}>{line.message}</color>";
            
            uiText.text += ">" + coloredMessage + "\n";

            Debug.Log("DIALOGUE: " + dialogueLines[currentMessageIndex].message);
            currentMessageIndex++;
        }
        else
        {
            timer = 0f;
            isActive = false;
        }
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
