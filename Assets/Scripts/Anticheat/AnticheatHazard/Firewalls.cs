using UnityEngine;

public class Firewalls : MonoBehaviour
{
    private bool wasActive = false;
    private GameObject antiCheat=null;
    void Start()
    {
        antiCheat = GameObject.FindWithTag("Anticheat");
        bool isCurrentlyActive = (antiCheat != null);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isCurrentlyActive);
        }
    }
    void Update()
    {
        antiCheat = GameObject.FindWithTag("Anticheat");
        bool isCurrentlyActive = (antiCheat != null);
        if (isCurrentlyActive != wasActive)
        {
            wasActive = isCurrentlyActive;
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isCurrentlyActive);
            }
            
        }
    }
}
