using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public static Table Instance { get; private set; }
    [SerializeField] private List<CardSO> _tableCards;
    [SerializeField] private Transform[] _cardHolders;
    [SerializeField] private int _nextCardIndex = 0;
    private int _openedCards = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AckardToTable(int cardCount)
    {
        CardSO[] cardsToTable = CardDealer.Instance.DealCardsToTable(cardCount);
        foreach (var card in cardsToTable)
        {
            _tableCards.Add(card);
            Debug.Log($"Masaya acilan kart: {card.Sign} - {card.Value}");
        }

    }
    public List<CardSO> GetCards()
    {
        return _tableCards;
    }
    public Transform NextCardHolder()
    {
        return _cardHolders[_nextCardIndex++];
    }
    public void OpenNextCard()
    {
        _cardHolders[_openedCards].GetChild(1).GetComponent<SpriteRenderer>().sprite = _tableCards[_openedCards].CardSprite;
        _cardHolders[_openedCards++].gameObject.SetActive(true);
    }
}
