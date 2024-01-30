using TMPro;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GridTile : MonoBehaviour
{

    public TileInfo info { get; private set; }
    public GridCell cell { get; private set; }
    public bool locked { get; set; }

    private Image background;
    private TextMeshProUGUI text;
    private float animDuration;
    private bool destroyedWithHammer;

    public void Init(GridCell cell, float _animDuration)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        animDuration = _animDuration;
        transform.position = cell.transform.position;
        GetComponent<RectTransform>().sizeDelta = cell.GetComponent<RectTransform>().sizeDelta;
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        Vector3 origScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(origScale, _animDuration);
    }

    public void SetState(TileInfo info, float scaleAnimDur = 0)
    {
        this.info = info;

        text.color = this.info.TextColor;
        text.text = this.info.Number.ToString();

        if (info.UserBackgroundSprite)
            background.sprite = info.BackgroundSprite;
        else
            background.color = this.info.BackgroundColor;

        if (scaleAnimDur > 0)
        {
            transform.DOScale(transform.localScale * 1.25f, scaleAnimDur).OnComplete(() => 
            {
                transform.DOScale(transform.localScale / 1.25f, scaleAnimDur);
            });
        }
    }

    public void MoveTo(GridCell cell)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.DOMove(cell.transform.position, animDuration);
    }

    public void Merge(GridCell cell, Action onMergeCompleted)
    {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

        transform.DOMove(cell.transform.position, animDuration).OnComplete(() =>
        {
            onMergeCompleted?.Invoke();
            Destroy(gameObject);
        });
    }

    public void ApplyHammer()
    {
        if (!GameManager.Instance.UiManager.HammerActive || destroyedWithHammer)
            return;

        destroyedWithHammer = true;
        transform.DOScale(Vector3.zero, 0.15f).OnComplete(() =>
        {
            GameManager.Instance.Board.RemoveTile(this);
            GameManager.Instance.UiManager.OnHammerApplied();
            Destroy(gameObject);
        });
    }

}