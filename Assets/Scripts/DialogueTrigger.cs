using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    public enum DialogueTarget { Terminal, Mover }

    [System.Serializable]
    public struct DialogueLine
    {
        public string message;
        public DialogueTarget target;
    }

    [Header("Dialogue Lines")]
    [SerializeField] private DialogueLine[] dialogueLines;

    [Header("Terminal Panel")]
    [SerializeField] private GameObject terminalBox;
    [SerializeField] private TextMeshProUGUI terminalText;

    [Header("Mover Panel")]
    [SerializeField] private GameObject moverBox;
    [SerializeField] private TextMeshProUGUI moverText;

    [Header("Behaviour")]
    [SerializeField] private bool triggerOnce = false;

    [Header("Timing")]
    [SerializeField] private float timeBetweenMessages = 2f;
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float cursorBlinkRate = 0.53f;
    [SerializeField] private float timeBeforeHide = 8f;

    private static DialogueTrigger s_active = null;
    private static DialogueTrigger s_pending = null;

    private bool hasTriggered = false;
    private int currentMessageIndex = 0;
    private float timer = 0f;
    private float cursorTimer = 0f;
    private bool isActive = false;
    private bool isTyping = false;
    private bool allLinesDone = false;
    private bool showCursor = false;

    // Terminal accumulates lines; mover shows one at a time
    private string terminalCompletedText = "";
    private string terminalCurrentLine = "";
    private string moverCurrentLine = "";

    private void Start()
    {
        if (terminalBox != null) terminalBox.SetActive(false);
        if (moverBox != null)   moverBox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && hasTriggered) return;
        if (isActive) return;

        if (s_active != null && !s_active.allLinesDone)
        {
            s_pending = this;
        }
        else
        {
            if (s_active != null) s_active.ForceStop();
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        hasTriggered = true;
        s_active = this;
        terminalCompletedText = "";
        terminalCurrentLine = "";
        moverCurrentLine = "";
        showCursor = false;
        cursorTimer = 0f;
        isActive = true;
        allLinesDone = false;
        currentMessageIndex = 0;
        timer = 0f;

        if (terminalText != null) terminalText.text = "";
        if (moverText != null)    moverText.text = "";

        ShowNextMessage();
    }

    private void ForceStop()
    {
        StopAllCoroutines();
        isActive = false;
        isTyping = false;
        showCursor = false;
        UpdateTerminalDisplay();
        if (terminalBox != null) terminalBox.SetActive(false);
        if (moverBox != null)    moverBox.SetActive(false);
        if (s_active == this) s_active = null;
    }

    private void Update()
    {
        if (isActive)
        {
            // Blink cursor on terminal panel
            cursorTimer += Time.deltaTime;
            if (cursorTimer >= cursorBlinkRate)
            {
                cursorTimer = 0f;
                showCursor = !showCursor;
                UpdateTerminalDisplay();
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
                        UpdateTerminalDisplay();
                        if (terminalBox != null) terminalBox.SetActive(false);
                        if (moverBox != null)    moverBox.SetActive(false);
                        if (s_active == this) s_active = null;
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
            DialogueLine line = dialogueLines[currentMessageIndex];
            Debug.Log("DIALOGUE: " + line.message);
            currentMessageIndex++;
            StartCoroutine(TypeLine(line));
        }
        else
        {
            allLinesDone = true;
            timer = 0f;

            if (s_pending != null)
            {
                DialogueTrigger next = s_pending;
                s_pending = null;
                ForceStop();
                next.StartDialogue();
            }
        }
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;

        if (line.target == DialogueTarget.Terminal)
        {
            if (terminalBox != null) terminalBox.SetActive(true);

            terminalCompletedText = "";
            terminalCurrentLine = "";
            if (terminalText != null) terminalText.text = "";
            showCursor = true;

            foreach (char c in line.message)
            {
                terminalCurrentLine += c;
                showCursor = true;
                UpdateTerminalDisplay();
                yield return new WaitForSeconds(typingSpeed);
            }

            terminalCompletedText = terminalCurrentLine;
            terminalCurrentLine = "";
            UpdateTerminalDisplay();
        }
        else // Mover
        {
            if (moverBox != null) moverBox.SetActive(true);

            moverCurrentLine = "";
            if (moverText != null) moverText.text = "";

            foreach (char c in line.message)
            {
                moverCurrentLine += c;
                if (moverText != null) moverText.text = moverCurrentLine;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;
    }

    private void UpdateTerminalDisplay()
    {
        if (terminalText != null)
            terminalText.text = terminalCompletedText + terminalCurrentLine + (showCursor ? "_" : "");
    }

#if UNITY_EDITOR
    [System.NonSerialized] private int _validatedLineCount = -1;

    private void OnValidate()
    {
        if (dialogueLines == null) return;

        if (_validatedLineCount == -1)
        {
            _validatedLineCount = dialogueLines.Length;
            return;
        }

        if (dialogueLines.Length > _validatedLineCount && _validatedLineCount >= 1)
        {
            int last = dialogueLines.Length - 1;
            DialogueLine newLine = dialogueLines[last];
            newLine.target = dialogueLines[last - 1].target == DialogueTarget.Terminal
                ? DialogueTarget.Mover
                : DialogueTarget.Terminal;
            dialogueLines[last] = newLine;
        }

        _validatedLineCount = dialogueLines.Length;
    }
#endif

    // Debug, print to console
    public void PrintDialogueToConsole()
    {
        foreach (DialogueLine line in dialogueLines)
        {
            Debug.Log("DIALOGUE [" + line.target + "]: " + line.message);
        }
    }
}
