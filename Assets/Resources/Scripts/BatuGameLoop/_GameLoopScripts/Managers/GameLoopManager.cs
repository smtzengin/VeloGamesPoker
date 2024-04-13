using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance { get; private set; }

    [SerializeField] private GameObject[] _players;
    private int _currentPlayerIndex = 0;
    private int _actionCount = 0;
    [SerializeField] private GameRound _currentRound;

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
        _currentRound = GameRound.PreFlop;
        LightManager.Instance.MoveTurnIndicator(_players[_currentPlayerIndex].transform.position);
    }

    public void OnPlayerAction()
    {
        Debug.Log($"Player {_currentPlayerIndex + 1} aksiyon aldi {_currentRound} roundunda.");

        _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Length;
        _actionCount++;

        if (_actionCount == _players.Length)
        {
            UpdateRound();
        }

        LightManager.Instance.MoveTurnIndicator(_players[_currentPlayerIndex].transform.position);

        Debug.Log($"Simdi Player {_currentPlayerIndex + 1}' in turu {_currentRound} roundunda.");
    }
    public GameObject[] GetPlayers()
    {
       Debug.Log("player return");
        return _players;
    }
    private void UpdateRound()
    {
        switch (_currentRound)
        {
            case GameRound.PreFlop:
                _currentRound = GameRound.Flop;
                Table.Instance.AckardToTable();
                break;
            case GameRound.Flop:
                _currentRound = GameRound.Turn;
                Table.Instance.AckardToTable();
                break;
            case GameRound.Turn:
                _currentRound = GameRound.River;
                Table.Instance.AckardToTable();
                break;
            case GameRound.River:
                _currentRound = GameRound.Showdown;
                break;
            case GameRound.Showdown:
                Debug.Log($"Turlar bitti/ Oyun bitti");
                break;
        }
        _actionCount = 0;
        Debug.Log($"Round suan {_currentRound}");
    }
}