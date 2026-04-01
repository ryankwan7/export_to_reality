using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is closed");
    }
}