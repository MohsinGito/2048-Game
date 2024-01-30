using UnityEngine;

public class BreathingEffect : MonoBehaviour
{

    public RectTransform targetRectTransform;
    public float breathDuration = 2f;
    public float scaleAmount = 0.02f;

    private Vector3 originalScale;
    private float currentTime;

    private void Start()
    {
        if (targetRectTransform != null)
        {
            originalScale = targetRectTransform.localScale;
        }
    }

    private void Update()
    {
        if (targetRectTransform != null)
        {
            float scaleFactor = 1 + Mathf.Sin((currentTime / breathDuration) * 2 * Mathf.PI) * scaleAmount;
            targetRectTransform.localScale = originalScale * scaleFactor;
            currentTime += Time.deltaTime;

            if (currentTime >= breathDuration)
            {
                currentTime = 0f;
            }
        }
    }

    private void OnDisable()
    {
        targetRectTransform.localScale = originalScale;
    }

}