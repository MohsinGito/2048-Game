using TMPro;
using System;
using UnityEngine;
using DG.Tweening;

public class ShopUi : MonoBehaviour
{

    public TMP_Text playerCoins;
    public GameObject watchAdButton;
    public CanvasGroup panelGroup;

    public Action OnClosed;

    public void Display()
    {
        playerCoins.text = SessionManager.Instance.PlayerCoins.ToString();
        watchAdButton.SetActive(SessionManager.Instance.IsAdAvailable());

        panelGroup.alpha = 0;
        panelGroup.gameObject.SetActive(true);
        panelGroup.DOFade(1, 0.25f);
    }
   
    public void Hide()
    {
        OnClosed?.Invoke();
        panelGroup.DOFade(0, 0.25f).OnComplete(() => panelGroup.gameObject.SetActive(false));
    }

    public void WatchAd()
    {
        if (!SessionManager.Instance.IsAdAvailable()) 
        {
            OverlayUiManager.Instance.InfoPopUp.Display("Ad Break Over for Today! More Coming Tomorrow!");
            return;
        }

        AdsManager.Instance.DisplayRewardedAd(() =>
        {
            SessionManager.Instance.ModifyCoins(150);
            SessionManager.Instance.TodaysAdViewed();
            OverlayUiManager.Instance.InfoPopUp.Display("Congrats! You've Earned 150 Coins!");

            watchAdButton.SetActive(false);
            playerCoins.text = SessionManager.Instance.PlayerCoins.ToString();
        }, () =>
        {
            OverlayUiManager.Instance.InfoPopUp.Display("Uable To View Ad!");
        } );
    }

    public void PurchaseItem(int itemId)
    {
        if (SessionManager.Instance.PlayerCoins < 150)
        {
            OverlayUiManager.Instance.InfoPopUp.Display("Oops! You Need More Coins!");
            return;
        }

        switch ((ShopItem)itemId)
        {
            case ShopItem.Hammer:
                SessionManager.Instance.ModifyHammarPowerup(1);
                break;
            case ShopItem.Multiplier:
                SessionManager.Instance.ModifyMultiplierPowerup(1);
                break;
        }

        SessionManager.Instance.ModifyCoins(-150);
        playerCoins.text = SessionManager.Instance.PlayerCoins.ToString(); 
        OverlayUiManager.Instance.InfoPopUp.Display("Purchase Successful!");
    }

}

public enum ShopItem
{
    Hammer = 0,
    Multiplier = 1
}