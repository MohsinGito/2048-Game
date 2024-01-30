using TMPro;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BonusGameUi : MonoBehaviour
{

    [Header("Slot Ui")]
    public List<Sprite> symbols;
    public List<SlotReel> reels;
    public Image symbolPrefab;
    public RectTransform startPos;
    public RectTransform resetPos;
    public float spinDuration;
    public float stopDuration;
    public float initialSpinSpeed;
    public int rewardCoins;
    public int totalAtempts;

    [Header("Other Ui")]
    public TMP_Text attemptsText;
    public TMP_Text informationText;
    public TMP_Text spinButtonText;
    public Button spinButton;
    public CanvasGroup panelFade;

    public Action OnClosed;

    private int attemptsLeft;
    private bool isSpinning = false;
    private float currentSpinSpeed;
    private const float AlignmentThreshold = 1f;

    private void Start()
    {
        foreach (SlotReel reel in reels)
        {
            var reelTopSymbol = Instantiate(symbolPrefab, reel.top);
            var reelMidSymbol = Instantiate(symbolPrefab, reel.mid);
            var reelBottomSymbol = Instantiate(symbolPrefab, reel.bottom);

            reelTopSymbol.sprite = symbols[UnityEngine.Random.Range(0, symbols.Count)];
            reelMidSymbol.sprite = symbols[UnityEngine.Random.Range(0, symbols.Count)];
            reelBottomSymbol.sprite = symbols[UnityEngine.Random.Range(0, symbols.Count)];
        }
    }

    public void Display()
    {
        panelFade.alpha = 0;
        attemptsLeft = totalAtempts;
        spinButtonText.text = "spin";
        attemptsText.text = $"You have {attemptsLeft} attempts";
        informationText.text = "To win, 3 identical chips must fall in a line";

        panelFade.gameObject.SetActive(true);
        spinButton.onClick.RemoveAllListeners();
        spinButton.onClick.AddListener(SpinReels);
        panelFade.DOFade(1, 0.5f);
    }

    public void Hide()
    {
        OnClosed?.Invoke();
        panelFade.DOFade(0, 0.5f).OnComplete(() =>
        {
            panelFade.gameObject.SetActive(false);
        });
    }

    private void SpinReels()
    {
        isSpinning = true;
        attemptsLeft -= 1;
        spinButton.interactable = false;
        currentSpinSpeed = initialSpinSpeed;
        attemptsText.text = $"You  Have {attemptsLeft} Attempts";
        informationText.text = "To Win, 3 Identical Chips Must Fall In A Line";

        StartCoroutine(StartSpinningReels());
        DOVirtual.DelayedCall(spinDuration, () =>
        {
            isSpinning = false;
        });
    }

    private IEnumerator StartSpinningReels()
    {
        yield return SpinWhile(() => isSpinning);
        yield return SlowDownSpin();
        yield return AlignSymbols();

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        bool allSymbolsMatch = reels.Select(reel => reel.mid.GetChild(0).GetComponent<Image>().sprite).Distinct().Count() == 1;

        if (allSymbolsMatch)
        {
            informationText.text = $"Congrats You Won {rewardCoins} Coins";
            SessionManager.Instance.AddCoins(rewardCoins);
        }
        else
        {
            if (attemptsLeft == 0)
                informationText.text = "Sorry, Try Again Tomorrow";
            else
                informationText.text = "Oops, You Can Try Again";
        }

        spinButton.interactable = true;
        if (attemptsLeft == 0)
        {
            spinButtonText.text = "menu";
            spinButton.onClick.RemoveAllListeners();
            spinButton.onClick.AddListener(Hide);
        }
    }

    private IEnumerator SpinWhile(Func<bool> condition)
    {
        while (condition())
        {
            SpinSymbols();
            yield return null;
        }
    }

    private IEnumerator SlowDownSpin()
    {
        float targetSpeed = initialSpinSpeed / 2;
        return AdjustSpinSpeed(() => currentSpinSpeed != targetSpeed, true);
    }

    private IEnumerator AlignSymbols()
    {
        return AdjustSpinSpeed(() => !IsAllSymbolsAligned(), true);
    }

    private IEnumerator AdjustSpinSpeed(Func<bool> condition, bool isSlowingDown)
    {
        while (condition())
        {
            currentSpinSpeed -= (initialSpinSpeed / stopDuration) * Time.deltaTime;
            currentSpinSpeed = Mathf.Max(currentSpinSpeed, initialSpinSpeed / 2);
            SpinSymbols(isSlowingDown);
            yield return null;
        }
    }

    private void SpinSymbols(bool isSlowingDown = false)
    {
        foreach (SlotReel reel in reels)
        {
            foreach (RectTransform symbolTransform in new RectTransform[] { reel.top, reel.mid, reel.bottom })
            {
                symbolTransform.Translate(0, -currentSpinSpeed * Time.deltaTime, 0, Space.World);
                ResetSymbolPosition(symbolTransform, isSlowingDown);
            }
        }
    }

    private void ResetSymbolPosition(RectTransform symbolTransform, bool isSlowingDown)
    {
        if (symbolTransform.position.y <= resetPos.position.y)
        {
            symbolTransform.position = new Vector2(symbolTransform.position.x, startPos.position.y);
            if (!isSlowingDown)
                symbolTransform.GetChild(0).GetComponent<Image>().sprite = symbols[UnityEngine.Random.Range(0, symbols.Count)];
        }
    }

    private bool IsAllSymbolsAligned()
    {
        foreach (SlotReel reel in reels)
        {
            if (!IsSymbolAligned(reel.top, 200) && !IsSymbolAligned(reel.mid, 0) && !IsSymbolAligned(reel.bottom, -200))
                return false;
        }

        foreach (SlotReel reel in reels)
        {
            reel.top.anchoredPosition = new Vector2(reel.top.anchoredPosition.x, 200);
            reel.mid.anchoredPosition = new Vector2(reel.mid.anchoredPosition.x, 0);
            reel.bottom.anchoredPosition = new Vector2(reel.bottom.anchoredPosition.x, -200);
        }

        return true;
    }

    private bool IsSymbolAligned(RectTransform symbolTransform, float expectedYPosition)
    {
        return Mathf.Abs(symbolTransform.anchoredPosition.y - expectedYPosition) < AlignmentThreshold;
    }

}

[System.Serializable]
public struct SlotReel
{
    public RectTransform top;
    public RectTransform mid;
    public RectTransform bottom;
}