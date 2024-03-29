using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance { get; private set; }

    [SerializeField] private GameObject[] _players; 
    private int currentPlayerIndex = 0;
    private int actionCount = 0;
    private GameRound _currentRound = GameRound.PreFlop;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LightManager.Instance.MoveTurnIndicator(_players[currentPlayerIndex].transform.position);
    }

    public void OnPlayerAction()
    {
        Debug.Log($"Player {currentPlayerIndex + 1} took an action in {_currentRound} round.");

        currentPlayerIndex = (currentPlayerIndex + 1) % _players.Length;
        actionCount++;

        if (actionCount == _players.Length)
        {
            UpdateRound();
        }

        LightManager.Instance.MoveTurnIndicator(_players[currentPlayerIndex].transform.position);

        Debug.Log($"It's now Player {currentPlayerIndex + 1}'s turn in {_currentRound} round.");
    }


    private void UpdateRound()
    {
        switch (_currentRound)
        {
            case GameRound.PreFlop:
                _currentRound = GameRound.Flop;
                break;
            case GameRound.Flop:
                _currentRound = GameRound.Turn;
                break;
            case GameRound.Turn:
                _currentRound = GameRound.River;
                break;
            case GameRound.River:
                _currentRound = GameRound.Showdown;
                break;
            case GameRound.Showdown:
                Debug.Log($"Game Over");
                break;
        }
        actionCount = 0;
        Debug.Log($"Round updated to {_currentRound}");
    }
}