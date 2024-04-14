using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public static Table Instance { get; private set; }
    [SerializeField] private List<CardSO> _tableCards;

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

    public void AckardToTable()
    {
        CardSO[] cardsToTable = CardDealer.Instance.DealCardsToTable(1);
        foreach (var card in cardsToTable)
        {
            _tableCards.Add(card);
            Debug.Log($"Masaya acilan kart: {card.Sign} - {card.Value}");
        }
    }
}
