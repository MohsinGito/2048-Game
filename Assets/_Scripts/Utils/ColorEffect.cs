using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ColorEffect : MonoBehaviour
{

    public enum ColorChangeType
    {
        SmoothTransition,
        Flashing,
        Gradual,
    }

    [Tooltip("Initial color of the image.")]
    public Color initialColor = Color.white;

    [Tooltip("Target color to transition to.")]
    public Color targetColor = Color.red;

    [Tooltip("The duration of the color transition.")]
    public float transitionDuration = 1f;

    [Tooltip("Type of color transition effect.")]
    public ColorChangeType colorChangeType = ColorChangeType.SmoothTransition;

    private Image imageComponent;

    private void Start()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.color = initialColor;
        Sequence colorSequence = DOTween.Sequence();

        switch (colorChangeType)
        {
            case ColorChangeType.SmoothTransition:
                colorSequence.Append(imageComponent.DOColor(targetColor, transitionDuration).SetEase(Ease.InOutSine));
                colorSequence.Append(imageComponent.DOColor(initialColor, transitionDuration).SetEase(Ease.InOutSine));
                break;
            case ColorChangeType.Flashing:
                colorSequence.Append(imageComponent.DOColor(targetColor, transitionDuration).SetEase(Ease.Flash));
                colorSequence.Append(imageComponent.DOColor(initialColor, transitionDuration).SetEase(Ease.Flash));
                break;
            case ColorChangeType.Gradual:
                colorSequence.Append(imageComponent.DOColor(targetColor, transitionDuration).SetEase(Ease.InQuad));
                colorSequence.Append(imageComponent.DOColor(initialColor, transitionDuration).SetEase(Ease.OutQuad));
                break;
        }

        colorSequence.SetLoops(-1, LoopType.Yoyo);
        colorSequence.Play();
    }

    private void OnDisable()
    {
        imageComponent.color = initialColor;
    }

    private void OnDestroy()
    {
        if (DOTween.IsTweening(imageComponent))
        {
            DOTween.Kill(imageComponent);
        }
    }

}