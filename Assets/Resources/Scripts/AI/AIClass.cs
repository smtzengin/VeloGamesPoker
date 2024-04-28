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
        float decision = Aggression * Mathf.Log(2,(HandCardPoint() + OnePairPoint()) / 2f) * Randomness * Random.Range(0.5f, 1.5f) / (Caution + Stupidity);
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

        PokerHand bestHand = combinations //tüm kombinasyonlar
            .Select(hand => PokerHandEvaluator.Instance.EvaluateHand(hand)) //handi alır degerlendirir
            .Aggregate((highest, next) => next > highest ? next : highest); //birlesitrme // en yüksek olani dondur

        Debug.Log((int)bestHand);

        float decision = Aggression * Mathf.Log(2, (HandCardPoint() + (int)bestHand) / 2f) * Randomness * Random.Range(0.5f, 1.5f) / (Caution + Stupidity);
        TryBet(decision);
    }

    private void TryBet(float decision)
    {
        if (GameLoopManager.Instance.InRoundTour != 3)
            if (decision >= RaiseThreshold)
                RaiseBet();
            else if (decision >= CallThreshold || GameLoopManager.Instance.InRoundTour == 0)
                CallCheckBet();
            else
                Fold();
        else
            CallCheckBet();
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
        Debug.Log("RAISED");
        Debug.Log("--------RAISE VALUES-------");
        if (Table.Instance.GetCards().Count < 3)
        {
            float handValue = HandCardPoint() / 3f; //Minimum 4/3, Maximum 28/3 puan
            Debug.Log(handValue);
            int raiseValue = Mathf.CeilToInt(handValue * 10 * Random.Range(0.2f, 1.501f) / 40) * 40;
            Debug.Log(raiseValue);

            if (raiseValue > GetChips())
                raiseValue = GetChips();
            if (raiseValue < 40)
                raiseValue = 40;
            ActionHelpers.Instance.Raise(this, raiseValue);
        }
        else
        {

        }
        Debug.Log("--------------------------");
    }
    private void CallCheckBet()
    {
        Debug.Log("CALLED/CHECKED");
        ActionHelpers.Instance.Call(this);
    }
    private void Fold()
    {
        Debug.Log("FOLD");
        ActionHelpers.Instance.Fold(this);
    }
}
