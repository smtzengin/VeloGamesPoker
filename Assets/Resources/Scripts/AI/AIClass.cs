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
            Debug.Log("Not enough cards on the table to evaluate a hand.");
            float decision = Aggression * (HandCardPoint() / 2) * Randomness * Random.Range(0.5f, 1.5f) / (Caution + Stupidity);
            if (GameLoopManager.Instance.InRoundTour != 3)
            {
                if (decision >= RaiseThreshold)
                    RaiseBet();
                else if (decision >= CallThreshold)
                    CallCheckBet();
                else if (GameLoopManager.Instance.InRoundTour == 0)
                    Fold();
                else
                    CallCheckBet();
            }
            else
                CallCheckBet();
            return;
        }

        List<List<CardSO>> combinations = PokerHandEvaluator.Instance.GenerateCombinations(Table.Instance.GetCards(), GetHand()); //olasi kombinasyonlar

        if (combinations.Count == 0) //kombinasyon yoksa
        {
            Debug.Log("No valid combinations could be generated.");
            return;
        }

        PokerHand bestHand = combinations //tüm kombinasyonlar
            .Select(hand => PokerHandEvaluator.Instance.EvaluateHand(hand)) //handi alır degerlendirir
            .Aggregate((highest, next) => next > highest ? next : highest); //birlesitrme // en yüksek olani dondur


        float handStrength = (float)bestHand / (float)PokerHand.RoyalFlush;  // Normalized hand strength
        float decisionFactor = Aggression * handStrength - Caution + Random.Range(-Randomness, Randomness);

        // Karar verme
        if (handStrength < 0.2 && decisionFactor < 0) // Zayıf el
        {
            AIBehaviors.Fold(this);
        }
        else if (handStrength > 0.7 || decisionFactor > 0.5) // Güçlü el veya yüksek agresyon
        {
            int betAmount = CalculateBetAmount(handStrength);
            AIBehaviors.Bet(this, betAmount);
        }
        else //call yok
        {
            AIBehaviors.Call(this);
        }
    }

    private int CalculateBetAmount(float handStrength)
    {
        int minBet = GameLoopManager.Instance.MinBid;
        int currentBid = GameLoopManager.Instance.CurrentBid;
        int raiseAmount = (int)(handStrength * (GetChips() * 0.25f)); // Örnek: el gücüne göre çiplerin %25'i kadar artır
        return Mathf.Max(minBet, raiseAmount);
    }

    private int HandCardPoint()
    {
        int value = 0;
        for (int i = 0; i < _hand.Count; i++)
            value += (int)_hand[i].Value;
        return value;
    }
    private void RaiseBet()
    {
        Debug.Log("RAISED");
        if (Table.Instance.GetCards().Count < 3)
        {
            Debug.Log("--------RAISE VALUES-------");
            float handValue = HandCardPoint() / 2f; //Minimum 2, Maximum 14 puan
            Debug.Log(handValue);
            int raiseValue = Mathf.CeilToInt(handValue * 10 * Random.Range(0.2f, 1.501f) / 40) * 40;
            Debug.Log(raiseValue);

            if (raiseValue > GetChips())
                raiseValue = GetChips();
            if (raiseValue < 40)
                raiseValue = 40;
            ActionHelpers.Instance.Raise(this, raiseValue);
            Debug.Log("--------------------------");
        }

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
