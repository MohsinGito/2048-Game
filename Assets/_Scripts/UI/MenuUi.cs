using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuUi : MonoBehaviour
{

    [SerializeField] private TMP_Text playerCoins;
    [SerializeField] private TMP_Text playerHighScores;
    [SerializeField] private Button bonsuGameButton;
    [SerializeField] private CanvasGroup menuUiPanel;

    private void Start()
    {
        playerCoins.text = SessionManager.Instance.PlayerCoins.ToString();
        playerHighScores.text = SessionManager.Instance.HighScores.ToString();
        bonsuGameButton.interactable = SessionManager.Instance.BonusGameAvailable();

        OverlayUiManager.Instance.ShopUi.OnClosed = () => 
        {
            menuUiPanel.DOFade(1, 0.25f);
            playerCoins.text = SessionManager.Instance.PlayerCoins.ToString();
        };
        OverlayUiManager.Instance.BonusGameUi.OnClosed = () => playerCoins.text = SessionManager.Instance.PlayerCoins.ToString();
    }

    public void OpenSettings()
    {
        OverlayUiManager.Instance.SettingsUI.Display();
    }

    public void OpenShop()
    {
        menuUiPanel.alpha = 0;
        OverlayUiManager.Instance.ShopUi.Display();
    }

    public void OpenBonusGame()
    {
        bonsuGameButton.interactable = false;
        SessionManager.Instance.TodaysBonusGamePlayed();
        OverlayUiManager.Instance.BonusGameUi.Display();
    }

    public void MoveToGameplayScene()
    {
        OverlayUiManager.Instance.LoadScene("Gameplay");
    }

}