using UnityEngine;

public class GameManager : MonoBehaviour
{

    [field: SerializeField] public BoardManager Board { private set; get; }
    [field: SerializeField] public UiManager UiManager { private set; get; }

    public int CurrentScores { get; set; }
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
    }

    public void NewGame()
    {
        CurrentScores = 0;
        Board.ClearBoard();
        Board.SetUpBoard();
        UiManager.OnGameStarted();
    }

    public void GameOver()
    {
        if (CurrentScores > SessionManager.Instance.HighScores)
            SessionManager.Instance.SaveNewHighScores(CurrentScores);

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
            SessionManager.Instance.AddCoins(25);
            UiManager.DisplayNewCoins();
        }
    }

}