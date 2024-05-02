using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Character { get; private set; }
    public bool IsFull { get; private set; } = false;

    [SerializeField] protected List<CardSO> _hand;
    [SerializeField] private int _chips = 2000;
    [SerializeField] private int _currentBet = 0;
    [SerializeField] private int _lastBet;
    [SerializeField] private Transform _dealerTransform;

    [SerializeField] private GameObject[] _onHandCards;
    private int _onHandCounter = 0;
    private PlayerAnimation _playerAnimation;
    public bool IsLocalPlayer { get { return _localPlayer; } }
    [SerializeField] private bool _localPlayer;

    [SerializeField] private PlayerCanvas _playerCanvas;

    private void Awake()
    {
        _hand = new List<CardSO>();
        //geçici olarak 2000 chip atýyoruz hepsine
        UpdateCanvas();
    }
    public void ReceiveCards(CardSO card)
    {
        _hand.Add(card);
        if (_localPlayer)
            UIManager.AddCard(card, true);
    }
    public void ClearHand()
    {
        _hand.Clear();
    }
    public List<CardSO> GetHand()
    {
        return _hand;
    }
    public int GetCurrentBid() { return _currentBet; }
    public int GetLastBid() { return _lastBet; }
    public void AddBid(int amount)
    {
        _playerAnimation.BidTrigger();
        _lastBet = amount;
        _currentBet += amount;
        DecreaseChips(amount);
        UpdateCanvas();
    }
    public void Check()
    {
        _playerAnimation.CheckTrigger();
    }
    public void Fold()
    {
        _playerAnimation.FoldTrigger();
    }
    public void BackToTable()
    {
        _playerAnimation.BackToTrigger();
    }
    public int GetChips() { return _chips; }
    public void DecreaseChips(int amount) { _chips -= amount; }
    public void IncreaseChips(int amount) { _chips += amount; }
    public void SetSeatTo(bool b) { IsFull = b; }
    public void AddVisualCardToHand()
    {
        _onHandCards[_onHandCounter].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = _hand[_onHandCounter].CardSprite;
        _onHandCards[_onHandCounter].SetActive(true);
        _onHandCounter = (_onHandCounter + 1) % 2;
    }
    public void SetVisualCards(GameObject[] visualCards) { _onHandCards = visualCards; }
    public void SetCharacter(GameObject character)
    {
        Character = character;
        if (Character != null)
            _playerAnimation = character.GetComponent<PlayerAnimation>();
    }

    //For Dealer Button and Chips
    public Transform GetDealerTransform() { return _dealerTransform; }
    public void ResetRoundBets()
    {
        _lastBet = 0;
        _currentBet = 0;
        UpdateCanvas();
    }
    private void UpdateCanvas()
    {
        _playerCanvas.UpdateBet(_currentBet);
        _playerCanvas.UpdateChips(_chips);
    }
    public void PlayerCanvasSetActive(bool active)
    {
        _playerCanvas.gameObject.SetActive(active);
    }
    public void ResetStats()
    {
        _currentBet = _lastBet = _onHandCounter = 0;
        for (int i = 0; i < _onHandCards.Length; i++)
            _onHandCards[i].SetActive(false);
        _hand.Clear();
    }

}
