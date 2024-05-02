using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance { get; private set; }

    [SerializeField] Player[] _allPlayers;
    [SerializeField] List<Player> _currentPlayers;

    private int _currentPlayerIndex = 0;
    private int _actionCount = 0;
    private int _totalBetAmount = 0;

    [SerializeField] private GameRound _currentRound;

    [SerializeField] private int _minBet = 0;
    [SerializeField] private int _currentBet;
    private bool _littleBet = false, _bigBet = false;
    private bool _roundDone = false;

    private int _lastDealer = 200;

    public int InRoundTour = 0;

    public void AddToTotalBet(int amount)
    {
        _totalBetAmount += amount;
    }
    public void ResetTotalBet()
    {
        _totalBetAmount = 0;
    }
    public int TotalBetAmount() {  return _totalBetAmount; }

    public int MinBid
    {
        get { return _minBet; }
        set { _minBet = value; }
    }
    public int CurrentBid
    {
        get { return _currentBet; }
        set
        {
            _currentBet = value;
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
    public void OnPlayerAction()
    {
        if (_currentPlayers.Count == 1)
        {
            UIManager.ToggleEndPanel(_currentPlayers[0].IsLocalPlayer);
            return; //winner
        }
        _actionCount++;

        if (_actionCount % _currentPlayers.Count == 0)
            InRoundTour++;

        if (_actionCount >= _currentPlayers.Count)
            CheckBids();

        if (_roundDone) return; //End Game
        UIManager.UpdateTableChipText(_currentBet);
        NextPlayer();
        //Check if it is ai's Turn.
        CheckTurn();
        LightManager.Instance.MoveTurnIndicator(_currentPlayers[_currentPlayerIndex].transform.position);
    }
    public void CheckBids()
    {
        if (_currentRound != GameRound.PreFlop || (_currentRound == GameRound.PreFlop && _actionCount > _currentPlayers.Count + 1))
        {
            Debug.Log(_currentPlayerIndex);
            if (_currentPlayerIndex >= _currentPlayers.Count) //If last player fold, return.
                return;
            int lastBid = _currentPlayers[_currentPlayerIndex].GetCurrentBid();
            if (_currentPlayers.TrueForAll((x) => x.GetCurrentBid() == lastBid))
                UpdateRound();
        }
    }
    public void RemovePlayer(Player p)
    {
        _currentPlayers.Remove(p);
        _actionCount =
            (_actionCount - 1 < 0) ? 0 : _actionCount - 1;

        _currentPlayerIndex = (_currentPlayerIndex + _currentPlayers.Count - 1) % _currentPlayers.Count;
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
        if (_currentRound != GameRound.Showdown)
        {
            _actionCount = 0;
            InRoundTour = 0;
            ResetRoundBet();
        }
        else _roundDone = true;

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
        _lastDealer = _lastDealer == 200 ? Random.Range(0, _allPlayers.Length) : _lastDealer;
        _currentPlayers = new List<Player>();

        for (int i = 0; i < _allPlayers.Length; i++)
            _currentPlayers.Add(_allPlayers[(_lastDealer + i) % _allPlayers.Length]);
        _currentPlayerIndex++;
        _lastDealer = (_lastDealer + 1) % _allPlayers.Length;
    }
    public Player GetCurrentPlayer()
    {
        return _currentPlayers[_currentPlayerIndex];
    }
    public Player GetLastPlayer()
    {
        return _currentPlayers[(_currentPlayerIndex + (_currentPlayers.Count - 1)) % _currentPlayers.Count];
    }
    private void ResetRoundBet()
    {
        _minBet = 0;
        for (int i = 0; i < _currentPlayers.Count; i++)
        {
            _currentPlayers[i].ResetRoundBets();
        }
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
                PokerHand currentHand = PokerHandEvaluator.Instance.EvaluateHand(combination); // Her bir kombinasyonu de�erlendir
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
                    if (CompareHands(combination, winnerHand) == 1)
                    {
                        winner = _currentPlayers[i];
                        winnerHand = combination;
                        bestHand = currentHand;
                    }

                if (currentHand > playersBestHand)
                    playersBestHand = currentHand;

            }
            Debug.Log($"Player {_currentPlayers[i].name}'s Hand: {playersBestHand}");

        }
        Debug.Log($"Winner: {winner} Hand: {bestHand}");

        UIManager.ToggleEndPanel(won: winner.IsLocalPlayer);
       
        if (winner.IsLocalPlayer)
        {
            Debug.Log("siz kazandiniz");
            Debug.Log(_totalBetAmount);
            winner.IncreaseChips(_totalBetAmount);
        }
    }
    private int CompareHands(List<CardSO> hand1, List<CardSO> hand2)
    {
        hand1.Sort((x, y) => x.Value.CompareTo(y.Value));
        hand2.Sort((x, y) => x.Value.CompareTo(y.Value));

        for (int i = hand1.Count - 1; i >= 0; i--) //Kartlari en y�ksekten en k�c�ge dogru karsilastir
        {
            if (hand1[i].Value > hand2[i].Value)
                return 1;
            else if (hand1[i].Value < hand2[i].Value)
                return -1;
        }
        return 0; //eller esit durumu
    }
    public void TryToStart(bool createNew)
    {
        if (_currentPlayers.TrueForAll(x => x.IsFull))
        {
            Debug.Log(_littleBet);
            _currentRound = GameRound.PreFlop;
            DistributeCards();
            return;
        }
        if (createNew)
            CheckPlayers();
    }
    private void CheckPlayers()
    {
        GameManager.Instance.CreateNewPlayer();
    }
    public void DistributeCards()
    {
        CardDealer.Instance.GiveDealerButton(_currentPlayers[0].GetDealerTransform());
    }
    public void NextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % _currentPlayers.Count;
    }
    void CheckTurn()
    {

        if (!AreFirstBetsDone())
            return;

        ActionHelpers.Instance.CheckChips(_currentPlayers[_currentPlayerIndex]);
        if (_currentPlayers[_currentPlayerIndex].IsLocalPlayer) //S�radaki karakter bizim karakter ise
            UIManager.AllButtonsActive(active: true);
        else
            _currentPlayers[_currentPlayerIndex].GetComponent<AIClass>().AIMakeDecision();

    }
    public void StartRound()
    {
        LightManager.Instance.MoveTurnIndicator(_currentPlayers[_currentPlayerIndex].transform.position);
        CheckTurn();
    }
    private bool AreFirstBetsDone()
    {
        if (!_littleBet)
        {
            UIManager.AllButtonsActive(active: false);
            ActionHelpers.Instance.Raise(_currentPlayers[_currentPlayerIndex], 20);
            _littleBet = true;
            return false;
        }
        else if (!_bigBet)
        {
            UIManager.AllButtonsActive(active: false);
            ActionHelpers.Instance.Raise(_currentPlayers[_currentPlayerIndex], 20);
            _bigBet = true;
            return false;
        }
        return true;
    }
    public void ResetGame()
    {
        _currentPlayerIndex = _actionCount = _currentBet = InRoundTour = _minBet = 0;
        SetupLine();
        _littleBet = _bigBet = _roundDone = false;
        Table.Instance.ResetGame();
        CardDealer.Instance.ResetGame();
        ChipsHandler.Instance.ResetGame();
        PlayersBackToTable();
        TryToStart(true);
        ResetTotalBet();

    }

    private void PlayersBackToTable()
    {
        for (int i = 0; i < _allPlayers.Length; i++)
        {
            _allPlayers[i].ResetStats();
            if (_allPlayers[i].Character != null && !_allPlayers[i].IsFull)
                _allPlayers[i].BackToTable();
        }
    }
}