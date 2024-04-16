using System.Collections.Generic;
using UnityEngine;

public class WinnerDeterminer : MonoBehaviour
{
    public static WinnerDeterminer Instance { get; private set; }

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

   /* public int DetermineWinner(List<Player> players, List<CardSO> tableCards) //Oyuncu elleri ve masa kartlarina göre
    {
        Debug.Log("try?");
        int winnerIndex = 0;
        PokerHand winningHand = PokerHand.HighCard;

        foreach (Player player in players) //oyuncu elleri icin döngü 
        {
            List<CardSO> hand = new List<CardSO>(player.GetHand()); //oyuncunun eliyle masa kartlari birlestir
            PokerHand currentHand = GameLoopManager.Instance.Deneme(player); //eli degerlendir 
            
            //hand.addrange(tablecards

            // En iyi eli belirle
            if (currentHand > winningHand)
            {
                winningHand = currentHand;
                winnerIndex = players.IndexOf(player);
            }
            // El esit cikarsa kartlari karsilastir
            else if (currentHand == winningHand)
            {
                List<CardSO> higherHand = new List<CardSO>(players[winnerIndex].GetHand());
                higherHand.AddRange(tableCards);
                if (CompareHands(hand, higherHand) == 1)
                {
                    winnerIndex = players.IndexOf(player);
                }
            }
            string newHand = "";
            for (int i = 0; i < hand.Count; i++)
            {
                newHand += hand[i].name + " ";
            }
            Debug.Log("WINNERHAND: " + newHand + " Hand Type: " + currentHand);
        }
        Debug.Log("Kazanan oyuncu: " + players[winnerIndex].name);
        List<CardSO> winner = new List<CardSO>(players[winnerIndex].GetHand());
        winner.AddRange(tableCards);

        string winHand = "";
        for (int i = 0; i < winner.Count; i++)
        {
            winHand += winner[i].name + " ";
        }
        Debug.Log("WINNERHAND: " + winHand);

        return winnerIndex;
    }
   */
    // Kartlari karsilastir kazanani belirle
    private int CompareHands(List<CardSO> hand1, List<CardSO> hand2)
    {
        hand1.Sort((x, y) => x.Value.CompareTo(y.Value));
        hand2.Sort((x, y) => x.Value.CompareTo(y.Value));

        for (int i = hand1.Count - 1; i >= 0; i--) //Kartlari en yüksekten en kücüge dogru karsilastir
        {
            if (hand1[i].Value > hand2[i].Value)
                return 1;
            else if (hand1[i].Value < hand2[i].Value)
                return -1;
        }
        return 0; //eller esit durumu
    }
}
