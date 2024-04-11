using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardDealer : MonoBehaviour
{
    public static CardDealer Instance { get; private set; }

    [SerializeField] private CardSO[] _cardSOs;
    [SerializeField] private List<CardSO> _remainingCards;

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

    private void Start()
    {
        _remainingCards = new List<CardSO>(_cardSOs);
        ShuffleCards();
        DealCardsToPlayers();
    }

    private void ShuffleCards()      //Fisher-Yates shuffle algoritması
    {
        for (int i = _remainingCards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardSO temp = _remainingCards[i];
            _remainingCards[i] = _remainingCards[randomIndex];
            _remainingCards[randomIndex] = temp;
        }
                Debug.Log("shuffle");
    }

    public void DealCardsToPlayers()
    {
        GameObject[] players = GameLoopManager.Instance.GetPlayers();
                Debug.Log("getplayers");

        int playerCount = players.Length;
        Debug.Log(" count" +  playerCount  + players.Length + " length");

        for (int i = 0; i < playerCount; i++)
        {
            List<CardSO> playerCards = new List<CardSO>();
            for (int j = 0; j < 2; j++)
            {
                int randomIndex = Random.Range(0, _remainingCards.Count);
                playerCards.Add(_remainingCards[randomIndex]);
                _remainingCards.RemoveAt(randomIndex);
            }
            players[i].GetComponent<Player>().ReceiveCards(playerCards.ToArray());

            Debug.Log($"Player {i + 1} 'in kartlari:");
            foreach (var card in playerCards)
            {
                Debug.Log($"Card: {card.Sign} - {card.Value}");
            }
        }
    }
}