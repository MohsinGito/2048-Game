using TMPro;
using UnityEngine;
using DG.Tweening;

public class InfoPopUp : MonoBehaviour
{

    [SerializeField] private float displayDuration;
    [SerializeField] private float animationDuration;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private CanvasGroup infoPanel;

    private Tween hideTween;
    private RectTransform rect;

    private void Awake()
    {
        rect = infoPanel.GetComponent<RectTransform>();
    }

    public void Display(string info)
    {
        if (hideTween != null)
            hideTween.Kill();

        hideTween = DOVirtual.DelayedCall(displayDuration, Hide);

        rect.DOKill();
        infoPanel.DOKill();

        infoPanel.alpha = 0;
        rect.anchoredPosition = new Vector2(0, -300);
        rect.DOAnchorPos(new Vector2(0, 200), animationDuration);
        infoPanel.DOFade(1, animationDuration);

        infoText.text = info;
    }

    private void Hide()
    {
        infoPanel.DOFade(0, animationDuration);
        rect.DOAnchorPos(new Vector2(0, -300), animationDuration);
    }

}