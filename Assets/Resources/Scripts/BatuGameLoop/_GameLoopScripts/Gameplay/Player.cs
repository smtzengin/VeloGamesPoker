using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   [SerializeField] private List<CardSO> _hand;

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
}
