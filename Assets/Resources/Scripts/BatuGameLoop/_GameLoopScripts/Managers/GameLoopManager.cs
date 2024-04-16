using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.XR;
using System.Numerics;
using Unity.VisualScripting;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance { get; private set; }

    [SerializeField] Player[] _allPlayer;
    [SerializeField] List<Player> _currentPlayers;

    private int _currentPlayerIndex = 0;
    private int _actionCount = 0;
    private int _loopCount = 0;

    [SerializeField] private GameRound _currentRound;

    [SerializeField] private int _minBid = 20;
    [SerializeField] private int _currentBid;
    public int MinBid
    {
        get { return _minBid; }
        set { _minBid = value; }
    }
    public int CurrentBid
    {
        get { return _currentBid; }
        set
        {
            _currentBid = value;
        }
    }

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
        SetupLine();
    }

    private void Start()
    {
        _currentRound = GameRound.PreFlop;
        LightManager.Instance.MoveTurnIndicator(_currentPlayers[_currentPlayerIndex].transform.position);
        ActionHelpers.Instance.FirstBids(20);
        ActionHelpers.Instance.FirstBids(20);
        //NOTE: First Player to bid freely is player[2]. player[0] and player[1] bid automatically by the game (20 and 40)
    }

    public void OnPlayerAction()
    {
        Debug.Log($"Player {_currentPlayerIndex + 1} aksiyon aldi {_currentRound} roundunda.");
        _actionCount++;
        if (_actionCount >= 4)
            CheckBids();

        _currentPlayerIndex = (_currentPlayerIndex + 1) % _currentPlayers.Count;


        LightManager.Instance.MoveTurnIndicator(_currentPlayers[_currentPlayerIndex].transform.position);

        Debug.Log($"Simdi Player {_currentPlayerIndex + 1}' in turu {_currentRound} roundunda.");
    }
    public void CheckBids()
    {
        if (_currentRound != GameRound.PreFlop || (_currentRound == GameRound.PreFlop && _actionCount > 6))
        {
            int lastBid = _currentPlayers[_currentPlayerIndex].GetCurrentBid();
            if (_currentPlayers.TrueForAll((x) => x.GetCurrentBid() == lastBid))
                UpdateRound();
        }
    }
    public void RemovePlayer(Player p)
    {
        _currentPlayers.Remove(p);
        _currentPlayerIndex--;
    }
    public List<Player> GetPlayers()
    {
        Debug.Log("player return");
        return _currentPlayers;
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
                Table.Instance.AckardToTable(3);
                break;
            case GameRound.Turn:
                _currentRound = GameRound.River;
                Table.Instance.AckardToTable(1);
                break;
            case GameRound.River:
                _currentRound = GameRound.Showdown;
                Table.Instance.AckardToTable(1);

                break;
            case GameRound.Showdown:
                Debug.Log($"Turlar bitti/ Oyun bitti");
                SelectBestCombination();
                break;
        }
        _actionCount = 0;
        Debug.Log($"Round suan {_currentRound}");
    }

    public void SetupLine()
    {
        int firstToStart = Random.Range(0, _allPlayer.Length);
        _currentPlayers = new List<Player>();

        for (int i = 0; i < _allPlayer.Length; i++)
            _currentPlayers.Add(_allPlayer[(firstToStart + i) % _allPlayer.Length]);

    }
    public Player GetCurrentPlayer()
    {
        return _currentPlayers[_currentPlayerIndex];
    }
    public void SelectBestCombination()
    {
        Player winner = null;
        List<CardSO> winnerHand = null;
        PokerHand bestHand = PokerHand.HighCard;

        for (int i = 0; i < _currentPlayers.Count; i++)
        {
            List<List<CardSO>> allCombinations = PokerHandEvaluator.Instance.GenerateCombinations(Table.Instance.GetCards(), _currentPlayers[i].GetHand());
            PokerHand playersBestHand = PokerHand.HighCard;
            foreach (List<CardSO> combination in allCombinations)
            {
                PokerHand currentHand = PokerHandEvaluator.Instance.EvaluateHand(combination); // Her bir kombinasyonu deðerlendir
                if (winnerHand == null)
                    winnerHand = combination;
                else if (currentHand > bestHand)
                {
                    winner = _currentPlayers[i];
                    winnerHand = combination;
                    bestHand = currentHand;
                }
                else if (currentHand == bestHand)
                {
                    if (CompareHands(combination, winnerHand) == 1)
                    {
                        winner = _currentPlayers[i];
                        winnerHand = combination;
                        bestHand = currentHand;
                    }
                }

                if(currentHand > playersBestHand)
                {
                    playersBestHand = currentHand;
                }

            }
            Debug.Log($"Player {_currentPlayers[i].name}'s Hand: {playersBestHand}");

        }
        Debug.Log($"Winner: {winner} Hand: {bestHand}");
        
    }
    private int CompareHands(List<CardSO> hand1, List<CardSO> hand2)
    {
        hand1.Sort((x, y) => x.Value.CompareTo(y.Value));
        hand2.Sort((x, y) => x.Value.CompareTo(y.Value));

        for (int i = hand1.Count - 1; i >= 0; i--) //Kartlari en yüksekten en kücüge dogru karsilastir
        {
            if (hand1[i].Value > hand2[i].Value)
                return 1;
            else if (hand1[i].Value < hand2[i].Value)
                return -1;
        }
        return 0; //eller esit durumu
    }
}