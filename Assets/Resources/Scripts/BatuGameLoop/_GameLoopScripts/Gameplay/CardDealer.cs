using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardDealer : MonoBehaviour
{
    public static CardDealer Instance { get; private set; }

    [SerializeField] private CardSO[] _cardSOs;
    [SerializeField] private List<CardSO> _remainingCards;
    [SerializeField] private Transform _cardSpawnPoint;
    [SerializeField] private GameObject _targetCard;
    [SerializeField] private DealerButton _dealerButton;
    private int _playerIndex;
    public Animator DealerAnimator { get; private set; }
    private int _cardsGiven = 0;
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
        DealerAnimator = GetComponent<Animator>();
        _remainingCards = new List<CardSO>(_cardSOs);
        ShuffleCards();
        //DealCardsToPlayers();

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
    }

    public CardSO[] DealCardsToTable(int count)
    {
        CardSO[] cardsToTable = new CardSO[count];
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, _remainingCards.Count);
            cardsToTable[i] = _remainingCards[randomIndex];

            TargetCard card = Instantiate(_targetCard, _cardSpawnPoint.position, Quaternion.identity).GetComponent<TargetCard>();
            card.Setup(Table.Instance.NextCardHolder());

            _remainingCards.RemoveAt(randomIndex);
        }
        return cardsToTable;
    }
    public void PlayDealAnimation()
    {
        DealerAnimator.SetBool("GiveCard", true);
    }
    private void DealToPlayer()
    {
        if (_cardsGiven >= GameLoopManager.Instance.GetCurrentPlayers().Count * 2)
        {
            DealerAnimator.SetBool("GiveCard", false);
            GameLoopManager.Instance.StartRound();
            return;
        }
        _cardsGiven++;
        Player p = GameLoopManager.Instance.GetCurrentPlayer();
        GameLoopManager.Instance.NextPlayer();
        int randomIndex = Random.Range(0, _remainingCards.Count);
        CardSO newCard = _remainingCards[randomIndex];
        p.ReceiveCards(newCard);
        _remainingCards.RemoveAt(randomIndex);

        TargetCard tCard = Instantiate(_targetCard, _cardSpawnPoint.position, Quaternion.identity).GetComponent<TargetCard>();
        tCard.Setup(p.transform);
    }
    public void GiveDealerButton(int playerIndex)
    {
        List<Player> players = GameLoopManager.Instance.GetCurrentPlayers();
        _dealerButton.SetTarget(players[playerIndex].GetDealerTransform());
        PlayDealAnimation();
    }


}