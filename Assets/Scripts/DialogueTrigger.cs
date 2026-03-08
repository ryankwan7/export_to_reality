using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // set string in Inspector
    [SerializeField] private string dialogueMessage = "Testing Testing I'm Just Suggesting";
    
    private bool hasTriggered = false;

    // Debug, print to console
    public void PrintDialogueToConsole()
    {
        if(!hasTriggered)
        {
            Debug.Log("DIALOGUE: " + dialogueMessage);
            hasTriggered = true;
        }
    }
}