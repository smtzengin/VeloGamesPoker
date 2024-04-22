using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Character { get; private set; }
    public bool IsFull { get; private set; } 
    
    [SerializeField] private List<CardSO> _hand;
    [SerializeField] private int _chips = 2000;
    [SerializeField] private int _currentBid;
    [SerializeField] private int _lastBid;
    [SerializeField] private Transform _dealerTransform;

    [SerializeField] private GameObject[] _onHandCards;
    private int _onHandCounter = 0;
    PlayerAnimation _playerAnimation;

    private void Awake()
    {
        _hand = new List<CardSO>();
    }

    public void ReceiveCards(CardSO card)
    {
        _hand.Add(card);
    }
    public void ClearHand()
    {
        _hand.Clear();
    }

    public List<CardSO> GetHand()
    {
        return _hand;
    }
    public int GetCurrentBid() { return _currentBid; }
    public int GetLastBid() { return _lastBid; }
    public void AddBid(int amount)
    {
        _playerAnimation.BidTrigger();
        _lastBid = amount;
        _currentBid += amount;
        DecreaseChips(amount);
    }
    public int GetChips() { return _chips; }
    public void DecreaseChips(int amount) { _chips -= amount; }
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
        _playerAnimation = character.GetComponent<PlayerAnimation>();
    }
    public Transform GetDealerTransform() { return _dealerTransform; }
}
