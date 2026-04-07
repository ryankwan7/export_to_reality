using UnityEngine;

public class FadeBackground : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float fadeDuration = 2.0f;
    
    private float timer = 0f;
    private bool isFinished = false;

    void Update()
    {
        // If we haven't finished fading, keep going
        if (!isFinished)
        {
            timer += Time.deltaTime;
            
            // Calculate alpha (from 1 down to 0)
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            
            // Apply the color
            Color c = spriteRenderer.color;
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);

            // Once timer hits the duration, stop updating
            if (timer >= fadeDuration)
            {
                isFinished = true;
                gameObject.SetActive(false); // Turn off the black screen
            }
        }
    }
}
