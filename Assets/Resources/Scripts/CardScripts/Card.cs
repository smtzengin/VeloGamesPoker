using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardSign
{
    Club, //Sinek
    Diamond, //Karo
    Heart, //Kalp
    Spade //Maça
}

public enum CardValue
{
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack, //Joker
    Queen, //Kız
    King, //Kral
    Ace //As
}

public enum CardColor
{
    Black,
    Red
}

public class Card : MonoBehaviour, IComparable<Card>
{
    [SerializeField] private CardSO card;
    public CardSO CardSO { get { return card; } }

    public Card SecondCard;


    private void Start()
    {
        
    }

    public CardColor GetCardColor()
    {
        if (card.Sign == CardSign.Diamond || card.Sign == CardSign.Heart)
            return CardColor.Red;
        else
            return CardColor.Black;
    }
    public string GetShortSign()
    {
        switch (card.Sign)
        {
            case CardSign.Spade:
                return "♠";
            case CardSign.Heart:
                return "♥";
            case CardSign.Diamond:
                return "♦";
            case CardSign.Club:
                return "♣";
            default:
                return "X";
        }
    }

    public string GetShortValue()
    {
        if (card.Value >= CardValue.Two && card.Value <= CardValue.Ten)
            //return ((int)this.Value).ToString();
            return Convert.ToString((int)card.Value);

        if (card.Value >= CardValue.Jack && card.Value <= CardValue.Ace)
        {
            string val = card.Value.ToString();
            return val[0].ToString();
        }

        return "Y";
    }

    public int CompareTo(Card other)
    {
        if ((int)card.Value == (int)other.CardSO.Value)
        {
            if ((int)card.Sign == (int)other.CardSO.Sign)
                return 0; //równe karty, w sumie niemożliwa sytuacja w grze

            return (int)card.Sign - (int)other.CardSO.Sign; //jeśli karta ma wyższy znak, zwraca liczbę dodatnią
        }

        return (int)card.Value - (int)other.CardSO.Value;
    }
    #region Operator Overloads
    //Operator overload
    public static bool operator ==(Card first, Card second)
    {
        if (first.CompareTo(second) == 0)
            return true;

        return false;
    }
    //Operator overload
    public static bool operator !=(Card first, Card second)
    {
        if (first.CompareTo(second) != 0)
            return true;

        return false;
    }
    //Operator overload
    public static bool operator >(Card first, Card second)
    {
        if (first.CompareTo(second) > 0)
            return true;

        return false;
    }
    //Operator overload
    public static bool operator <(Card first, Card second)
    {
        if (first.CompareTo(second) < 0)
            return true;

        return false;
    } 
    #endregion
}
