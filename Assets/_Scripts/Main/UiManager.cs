using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    [Header("Gamplay UI")]
    [SerializeField] private TMP_Text displayCoins;
    [SerializeField] private TMP_Text displayHighScores;
    [SerializeField] private TMP_Text displayCurrentScores;

    [Header("Gameplay Panels UI")]
    [SerializeField] private TMP_Text gameOverHighScores;
    [SerializeField] private TMP_Text gameOverCurrentScores;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pauseGamePanel;

    [Header("Gameplay PowerUps")]
    [SerializeField] private TMP_Text hammerPowerupCount;
    [SerializeField] private TMP_Text multiplierPowerupCount;
    [SerializeField] private Button hammerPowerupButton;
    [SerializeField] private Button mutliplierPowerupButton;
    [SerializeField] private Image multiplierFill;

    public bool MultiplierActive { private set; get; }
    public bool HammerActive { private set; get; }

    #region Public Methods

    public void OnGameStarted()
    {
        gameOverPanel.SetActive(false);
        pauseGamePanel.SetActive(false);

        GameManager.Instance.Board.InputPaused = false;
        displayCoins.text = SessionManager.Instance.PlayerCoins.ToString();
        displayHighScores.text = SessionManager.Instance.HighScores.ToString();
        displayCurrentScores.text = GameManager.Instance.CurrentScores.ToString();
        hammerPowerupCount.text = SessionManager.Instance.HammarCount.ToString();
        multiplierPowerupCount.text = SessionManager.Instance.MultiplierCount.ToString();
    }

    public void OnGameEnded()
    {
        pauseGamePanel.SetActive(false);
        GameManager.Instance.Board.InputPaused = true;
        DOVirtual.DelayedCall(1f, () => gameOverPanel.SetActive(true));

        gameOverHighScores.text = SessionManager.Instance.HighScores.ToString();
        gameOverCurrentScores.text = GameManager.Instance.CurrentScores.ToString();
    }

    public void DisplayNewScores()
    {
        displayCurrentScores.text = GameManager.Instance.CurrentScores.ToString();
    }

    public void DisplayNewCoins()
    {
        displayCoins.text = SessionManager.Instance.PlayerCoins.ToString();
    }

    public void OnHammerApplied()
    {
        HammerActive = false;
        GameManager.Instance.Board.InputPaused = false;
        hammerPowerupButton.interactable = SessionManager.Instance.HammarCount > 0;
    }

    #endregion

    #region Buttons Methods

    public void ReturnToMain()
    {
        OverlayUiManager.Instance.LoadScene("Menu");
    }

    public void RestartGame()
    {
        GameManager.Instance.Board.InputPaused = false;
        GameManager.Instance.NewGame();
    }

    public void PauseGame()
    {
        pauseGamePanel.SetActive(true);
        GameManager.Instance.Board.InputPaused = true;
    }

    public void ResumeGame()
    {
        pauseGamePanel.SetActive(false);
        GameManager.Instance.Board.InputPaused = false;
    }
    
    public void OpenSettings()
    {
        OverlayUiManager.Instance.SettingsUI.Display();
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        hammerPowerupButton.onClick.AddListener(ActivateHammerPowerup);
        mutliplierPowerupButton.onClick.AddListener(ActivateMultiplierPowerup);

        hammerPowerupButton.interactable = SessionManager.Instance.HammarCount > 0;
        mutliplierPowerupButton.interactable = SessionManager.Instance.MultiplierCount > 0;
    }

    private void ActivateHammerPowerup()
    {
        if (!GameManager.Instance.Board.CanActivateHammer())
        {
            OverlayUiManager.Instance.InfoPopUp.Display("To Activate Hammer, Board Must Have More Than One Tile");
            return;
        }

        HammerActive = true;
        hammerPowerupButton.interactable = false;
        GameManager.Instance.Board.InputPaused = true;
        SessionManager.Instance.ModifyHammarPowerup(-1);
        hammerPowerupCount.text = SessionManager.Instance.HammarCount.ToString();
    }

    private void ActivateMultiplierPowerup()
    {
        if (MultiplierActive || SessionManager.Instance.MultiplierCount == 0)
            return;

        SessionManager.Instance.ModifyMultiplierPowerup(-1);
        multiplierPowerupCount.text = SessionManager.Instance.MultiplierCount.ToString();

        MultiplierActive = true;
        multiplierFill.fillAmount = 1;
        multiplierFill.gameObject.SetActive(true);
        multiplierFill.DOFillAmount(0, 30f).OnComplete(() =>
        {
            MultiplierActive = false;
            mutliplierPowerupButton.interactable = SessionManager.Instance.MultiplierCount > 0;
            multiplierFill.gameObject.SetActive(false);
        });
    }

    #endregion

}