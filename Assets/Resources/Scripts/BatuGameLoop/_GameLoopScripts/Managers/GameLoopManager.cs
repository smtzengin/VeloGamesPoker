using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance { get; private set; }

    [SerializeField] Player[] _allPlayers;
    [SerializeField] List<Player> _currentPlayers;

    private int _currentPlayerIndex = 0;
    private int _actionCount = 0;

    [SerializeField] private GameRound _currentRound;

    [SerializeField] private int _minBid = 20;
    [SerializeField] private int _currentBid;
    private int _dealerButtonIndex;
    private bool _littleBid = false, _bigBid = false;

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
        //LightManager.Instance.MoveTurnIndicator(_currentPlayers[_currentPlayerIndex].transform.position);
        //ActionHelpers.Instance.FirstBids(20);
        //ActionHelpers.Instance.FirstBids(20);
    }

    public void OnPlayerAction()
    {
        Debug.Log($"Player {_currentPlayerIndex + 1} aksiyon aldi {_currentRound} roundunda.");
        _actionCount++;
        if (_actionCount >= 4)
            CheckBids();

        NextPlayer();
        //Check if it is ai's Turn.
        CheckTurn();
        LightManager.Instance.MoveTurnIndicator(_currentPlayers[_currentPlayerIndex].transform.position);

        Debug.Log($"Simdi Player {_currentPlayerIndex + 1}' in turu {_currentRound} roundunda.");
    }
    public void CheckBids()
    {
        if (_currentRound != GameRound.PreFlop || (_currentRound == GameRound.PreFlop && _actionCount > 5))
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
    public List<Player> GetCurrentPlayers()
    {
        return _currentPlayers;
    }
    public Player[] GetPlayersInLine()
    {
        return _allPlayers;
    }
    private void UpdateRound()
    {
        switch (_currentRound)
        {
            case GameRound.PreFlop:
                ChipsHandler.Instance.NextGameLoop();
                _currentRound = GameRound.Flop;
                break;
            case GameRound.Flop:
                ChipsHandler.Instance.NextGameLoop();
                _currentRound = GameRound.Turn;
                break;
            case GameRound.Turn:
                ChipsHandler.Instance.NextGameLoop();
                _currentRound = GameRound.River;
                break;
            case GameRound.River:
                ChipsHandler.Instance.NextGameLoop();
                _currentRound = GameRound.Showdown;
                break;
            case GameRound.Showdown:
                Debug.Log($"Turlar bitti/ Oyun bitti");
                SelectBestCombination();
                break;
        }
        _actionCount = 0;
        Debug.Log($"Round suan {_currentRound}");
    }
    public void Ackard()
    {
        if (_currentRound == GameRound.Flop)
            Table.Instance.AckardToTable(3);
        if (_currentRound == GameRound.Turn)
            Table.Instance.AckardToTable(1);
        if (_currentRound == GameRound.River)
            Table.Instance.AckardToTable(1);
    }
    public void SetupLine()
    {
        int firstToStart = Random.Range(0, _allPlayers.Length);
        _currentPlayers = new List<Player>();

        for (int i = 0; i < _allPlayers.Length; i++)
            _currentPlayers.Add(_allPlayers[(firstToStart + i) % _allPlayers.Length]);
        _dealerButtonIndex = 0;
        _currentPlayerIndex++;
    }
    public void NextDealer()
    {
        _dealerButtonIndex++;
        _currentPlayerIndex = _dealerButtonIndex + 1;
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
                {
                    bestHand = currentHand;
                    winnerHand = combination;
                }
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

                if (currentHand > playersBestHand)
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
    public void TryToStart()
    {
        if (_currentPlayers.TrueForAll(x => x.IsFull))
            DistributeCards();
    }
    public void DistributeCards()
    {
        CardDealer.Instance.GiveDealerButton(_dealerButtonIndex);
        CardDealer.Instance.PlayDealAnimation();
    }
    public void NextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _currentPlayers.Count;
    }
    void CheckTurn()
    {
        if (!_littleBid)
        {
            ActionHelpers.Instance.Raise(_currentPlayers[_currentPlayerIndex], 20);
            _littleBid = true;
            return;
        }
        else if (!_bigBid)
        {
            ActionHelpers.Instance.Raise(_currentPlayers[_currentPlayerIndex], 20);
            _bigBid = true;
            return;
        }

        if (_currentPlayers[_currentPlayerIndex].CompareTag("Player") && _minBid != 0)
            ActionHelpers.Instance.SetInteraction(true);
        else
        {
            ActionHelpers.Instance.SetInteraction(false);
            //Get AI Choose
        }
    }
    public void StartRound()
    {
        LightManager.Instance.MoveTurnIndicator(_currentPlayers[_currentPlayerIndex].transform.position);
        CheckTurn();
    }
}