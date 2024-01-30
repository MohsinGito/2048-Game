using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadingEffect : MonoBehaviour
{

    [Tooltip("The duration of time it takes to fade in or out.")]
    public float fadeDuration = 1f;

    [Tooltip("The minimum alpha value.")]
    [Range(0f, 1f)]
    public float startingAlpha = 0f;

    [Tooltip("The maximum alpha value.")]
    [Range(0f, 1f)]
    public float targetAlpha = 1f;

    private CanvasGroup canvasGroup;
    private float fadeSpeed;
    private bool isFadingToTarget = true;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        canvasGroup.alpha = startingAlpha;
        fadeSpeed = Mathf.Abs(targetAlpha - startingAlpha) / fadeDuration;
    }

    private void Update()
    {
        float alphaTarget = isFadingToTarget ? targetAlpha : startingAlpha;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alphaTarget, fadeSpeed * Time.deltaTime);

        if (Mathf.Approximately(canvasGroup.alpha, alphaTarget))
        {
            isFadingToTarget = !isFadingToTarget;
            fadeSpeed = Mathf.Abs(targetAlpha - startingAlpha) / fadeDuration;
        }
    }

    private void OnDisable()
    {
        canvasGroup.alpha = targetAlpha;
    }

}