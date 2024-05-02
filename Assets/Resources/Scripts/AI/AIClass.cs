using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIClass : Player
{
    public float Aggression; //agresiflik
    public float Caution; //dikkat
    public float Stupidity; //aptallik
    public float Randomness; //rastgele icin
    public float RaiseThreshold; // threshold for raise
    public float CallThreshold; // threshold for raise
    private PokerHand _bestHand;
    public void AIMakeDecision()
    {
        if (Table.Instance.GetCards().Count < 3)
        {
            BeforeCards();
            return;
        }

        AfterCards();
    }
    private void BeforeCards()
    {
        float rand = Random.Range(0.5f, 1.5f);
        float decision = Aggression * Mathf.Log((HandCardPoint() + OnePairPoint()) / 2f, 2) * Randomness * rand / (Caution + Stupidity);

        TryBet(decision);
    }
    private void AfterCards()
    {
        List<List<CardSO>> combinations = PokerHandEvaluator.Instance.GenerateCombinations(Table.Instance.GetCards(), GetHand()); //olasi kombinasyonlar

        if (combinations.Count == 0) //kombinasyon yoksa (imkansız ?)
        {
            BeforeCards();
            return;
        }

        _bestHand = combinations //tüm kombinasyonlar
            .Select(hand => PokerHandEvaluator.Instance.EvaluateHand(hand)) //handi alır degerlendirir
            .Aggregate((highest, next) => next > highest ? next : highest); //birlesitrme // en yüksek olani dondur

        float rand = Random.Range(0.5f, 1.5f);
        float decision = Aggression * Mathf.Log((HandCardPoint() + (int)_bestHand) / 2f, 2) * Randomness * rand / (Caution + Stupidity);

        TryBet(decision);
    }

    private void TryBet(float decision)
    {
        if (GameLoopManager.Instance.InRoundTour < 3)
        {
            if (decision >= RaiseThreshold)
                RaiseBet();
            else if (decision >= CallThreshold || GameLoopManager.Instance.InRoundTour == 0)
                CallCheckBet();
            else
                Fold();
        }
        else
        {
            CallCheckBet();
        }
    }

    private int HandCardPoint()
    {
        int value = 0;
        for (int i = 0; i < _hand.Count; i++)
            value += (int)_hand[i].Value;
        return value;
    }
    private int OnePairPoint()
    {
        return PokerHandEvaluator.Instance.HasPair(_hand) ? 10 : 0;
    }
    private void RaiseBet()
    {
        int raiseValue;
        if (Table.Instance.GetCards().Count < 3)
        {
            float handValue = HandCardPoint() / 3f; //Minimum 4/3, Maximum 28/3 puan
            raiseValue = Mathf.CeilToInt(handValue * 2 * Random.Range(0.2f, 1.35f) / 40) * 40; 
        }
        else
        {
            int pokerHand = (int)_bestHand;
            float rand = Random.Range(0.2f, 1.35f);
            raiseValue = Mathf.CeilToInt(pokerHand * 3 * rand / 40) * 40;
            Debug.Log($"RaisedValue: {raiseValue} Rand: {rand}  CeilToInt: {Mathf.CeilToInt(pokerHand * 5 * rand / 40)} INSIDE: {pokerHand * 5 * rand / 40}");
        }
        if (raiseValue > GetChips())
            raiseValue = GetChips();
        if (raiseValue < 40)
            raiseValue = 40;
        ActionHelpers.Instance.Raise(this, raiseValue);
    }
    private void CallCheckBet()
    {
        ActionHelpers.Instance.Call(this);
    }
}
