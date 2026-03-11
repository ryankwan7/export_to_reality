using UnityEngine;

public class AfterImage : MonoBehaviour
{
private SpriteRenderer sr;
    private float alpha;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private Color color = Color.red;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        alpha = 1f;
    }

    void Update()
    {
        alpha -= fadeSpeed * Time.deltaTime;
        sr.color = new Color(color.r, color.g, color.b, alpha);

        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
        transform.position += (Vector3)Random.insideUnitCircle * 0.02f;
    }
}
