using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    // set string in Inspector
    [SerializeField] private string dialogueMessage = "Testing Testing I'm Just Suggesting";
    [SerializeField] private TextMeshProUGUI uiText;

    private bool hasTriggered = false;

    // Debug, print to console
    public void PrintDialogueToConsole()
    {
        if(!hasTriggered)
        {
            uiText.text = dialogueMessage;
            Debug.Log("DIALOGUE: " + dialogueMessage);
            hasTriggered = true;
        }
    }
}