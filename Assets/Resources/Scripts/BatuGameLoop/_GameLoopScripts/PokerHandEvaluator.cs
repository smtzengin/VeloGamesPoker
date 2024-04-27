using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PokerHandEvaluator : MonoBehaviour
{
    public static PokerHandEvaluator Instance { get; private set; }

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

    public PokerHand EvaluateHand(List<CardSO> hand)
    {
        //hand.Sort((x, y) => x.Value.CompareTo(y.Value)); //eli kart degerine göre siralicak

        if (IsRoyalFlush(hand)) //azalan siralamayla her olasi eli kontrol edicek
            return PokerHand.RoyalFlush;
        if (IsStraightFlush(hand))
            return PokerHand.StraightFlush;
        if (IsFourOfAKind(hand))
            return PokerHand.FourOfAKind;
        if (IsFullHouse(hand))
            return PokerHand.FullHouse;
        if (IsFlush(hand))
            return PokerHand.Flush;
        if (IsStraight(hand))
            return PokerHand.Straight;
        if (IsThreeOfAKind(hand))
            return PokerHand.ThreeOfAKind;
        if (IsTwoPair(hand))
            return PokerHand.TwoPair;
        if (IsPair(hand))
            return PokerHand.Pair;

        return PokerHand.HighCard; //hic durrum yoksa highcarda göre secim
    }

    private bool IsRoyalFlush(List<CardSO> hand) //RF mi ? 10 dan ASa kadar var mı?
    {
        if (IsStraightFlush(hand) && hand[0].Value == CardValue.Ten && hand[4].Value == CardValue.Ace)
            return true;
        return false;
    }

    private bool IsStraightFlush(List<CardSO> hand) //Tüm kartlar ayni renk sirali mi ?
    {
        if (IsFlush(hand) && IsStraight(hand))
            return true;
        return false;
    }

    private bool IsFourOfAKind(List<CardSO> hand) //Aynı degere sahip dort kart mi ? 
    {
        for (int i = 0; i <= hand.Count - 4; i++)
        {
            if (hand[i].Value == hand[i + 1].Value && hand[i + 1].Value == hand[i + 2].Value && hand[i + 2].Value == hand[i + 3].Value)
                return true;
        }
        return false;
    }

    private bool IsFullHouse(List<CardSO> hand) //üc kart ve iki kart
    {
        if ((hand[0].Value == hand[1].Value && hand[1].Value == hand[2].Value && hand[3].Value == hand[4].Value) ||
            (hand[0].Value == hand[1].Value && hand[2].Value == hand[3].Value && hand[3].Value == hand[4].Value))
            return true;
        return false;
    }

    private bool IsFlush(List<CardSO> hand) //tüm kartlar ayni renk mi
    {
        for (int i = 1; i < hand.Count; i++)
        {
            if (hand[i].Sign != hand[0].Sign)
                return false;
        }
        return true;
    }

    private bool IsStraight(List<CardSO> hand) //tüm kartlar sirali mi
    {
        for (int i = 1; i < hand.Count; i++)
        {
            if (hand[i].Value != hand[i - 1].Value + 1)
                return false;
        }
        return true;
    }

    private bool IsThreeOfAKind(List<CardSO> hand) //ayni degere sahip 3 kart
    {

        for (int i = 0; i <= hand.Count - 3; i++)
        {
            if (hand[i].Value == hand[i + 1].Value && hand[i + 1].Value == hand[i + 2].Value)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsTwoPair(List<CardSO> hand) //el 2 per
    {
        int pairCount = 0;
        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (hand[i].Value == hand[i + 1].Value)
            {
                pairCount++;
                i++;
            }
        }
        if (pairCount == 2)
            return true;
        return false;
    }

    private bool IsPair(List<CardSO> hand) //el per
    {
        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (hand[i].Value == hand[i + 1].Value)
                return true;
        }
        return false;
    }

    public List<List<CardSO>> GenerateCombinations(List<CardSO> tableCards, List<CardSO> playerHand)
    {
        List<List<CardSO>> combinations = new List<List<CardSO>>();
        for (int primaryCard = 0; primaryCard < tableCards.Count; primaryCard++) // tüm kartlar için
            for (int nextCard = 1; nextCard < 4; nextCard++) // 3 kez döndür
            {
                List<CardSO> newCombination = new List<CardSO> { tableCards[primaryCard] };

                for (int j = primaryCard + nextCard; j < primaryCard + nextCard + 2; j++)
                    newCombination.Add(tableCards[j % tableCards.Count]);

                newCombination.AddRange(playerHand);
                newCombination.Sort((x, y) => x.Value.CompareTo(y.Value));

                if (!AreCombinationsSame(newCombination, combinations))
                    combinations.Add(newCombination);

            }

        if (Table.Instance.GetCards().Count == 5)
        {
            List<CardSO> lastCombination = new List<CardSO>
            {
                tableCards[0],
                tableCards[2],
                tableCards[4],
                playerHand[0],
                playerHand[1],
            };
            combinations.Add(lastCombination);
        }

        return combinations;
    }
    bool AreCombinationsSame(List<CardSO> currentCombination, List<List<CardSO>> allCombinations)
    {
        if (allCombinations.Count == 0 || !allCombinations.Contains(currentCombination))
            return false;
        return true;
    }

}

