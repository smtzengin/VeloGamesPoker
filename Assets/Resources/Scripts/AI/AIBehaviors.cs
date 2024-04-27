using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviors : MonoBehaviour
{
    public static void Fold(Player player)
    {
        Debug.Log($"{player.gameObject.name} folds.");
    }

    public static void Call(Player player)
    {
        int callAmount = player.GetCurrentBid() - player.GetLastBid();
        if (player.GetChips() >= callAmount)
        {
            player.AddBid(callAmount);
            Debug.Log($"{player.gameObject.name} calls.");
        }
        else
        {
            Fold(player);
        }
    }

    public static void Bet(Player player, int betAmount)
    {
        if (player.GetChips() >= betAmount)
        {
            player.AddBid(betAmount);
            Debug.Log($"{player.gameObject.name} bets {betAmount}.");
        }
        else
        {
            Call(player); 
        }
    }
}
