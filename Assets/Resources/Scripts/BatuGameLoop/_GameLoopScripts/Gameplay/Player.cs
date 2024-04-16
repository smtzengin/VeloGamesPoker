using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<CardSO> _hand;
    [SerializeField] private int _chips = 2000;
    [SerializeField] private int _currentBid;
    [SerializeField] private int _lastBid;

    private void Start()
    {
        _hand = new List<CardSO>();
    }

    public void ReceiveCards(CardSO[] cards)
    {
        _hand.AddRange(cards);
        // Debug.Log($"Player {gameObject.name} kartlari:");
        // foreach (var card in cards)
        // {
        //     Debug.Log($"Card: {card.Sign} - {card.Value}");
        // }
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
        _lastBid = amount;
        _currentBid += amount;
        DecreaseChips(amount);
    }
    public int GetChips() { return _chips; }
    public void DecreaseChips(int amount) { _chips -= amount; }
}
