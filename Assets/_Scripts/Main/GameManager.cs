using UnityEngine;
using Utilities.Audio;

public class GameManager : MonoBehaviour
{

    [field: SerializeField] public BoardManager Board { private set; get; }
    [field: SerializeField] public UiManager UiManager { private set; get; }

    public int CurrentScores { get; set; }
    public bool CanContinue { get; set; }
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start()
    {
        Board.Init();
        NewGame();

        AudioController.Instance.PlayAudio(AudioName.GAMEPLAY_MUSIC);
    }

    public void NewGame(bool continuedGame = false)
    {
        CurrentScores = continuedGame ? CurrentScores : 0;

        Board.ClearBoard();
        Board.SetUpBoard();
        UiManager.OnGameStarted();
    }

    public void GameOver()
    {
        if (CurrentScores > SessionManager.Instance.HighScores)
            SessionManager.Instance.SaveNewHighScores(CurrentScores);

        CanContinue = !CanContinue;
        Board.InputPaused = true;
        UiManager.OnGameEnded();
    }

    public void IncreaseScore(int newScores)
    {
        if (UiManager.MultiplierActive)
            newScores *= 2;

        CurrentScores += newScores;
        UiManager.DisplayNewScores();

        if (newScores > 2048)
        {
            SessionManager.Instance.ModifyCoins(25);
            AudioController.Instance.PlayAudio(AudioName.COINS_BONUS);
            UiManager.DisplayNewCoins();
        }
    }

}
