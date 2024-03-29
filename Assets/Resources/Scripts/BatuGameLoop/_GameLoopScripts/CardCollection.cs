using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardCollection
{}
// {

//     public List<Card> Cards { get; set; }

//     public CardCollection()
//     {
//         Cards = new List<Card>();
//     }

//     public CardCollection(List<Card> cards)
//     {
//         Cards = cards;
//     }

//     // Kart eklemek için bir metot
//     public bool AddCard(Card card)
//     {
//         if (Cards == null)
//             Cards = new List<Card>();

//         Cards.Add(card);
//         return true;
//     }

//     // Belirli bir işarete ve değere sahip kartı çıkarmak için bir metot
//     public Card TakeOutCard(CardSign sign, CardValue value)
//     {
//         if (Cards == null)
//             return null;

//         var takenCard = Cards.FirstOrDefault(c => c.CardSO.Sign == sign && c.CardSO.Value == value);
//         if (takenCard != null)
//             Cards.Remove(takenCard);
//         return takenCard;
//     }

//     // Belirli bir indekse sahip kartı çıkarmak için bir metot
//     public Card TakeOutCard(int index)
//     {
//         if (Cards == null || index < 0 || index >= Cards.Count)
//             return null;

//         var takenCard = Cards[index];
//         Cards.RemoveAt(index);
//         return takenCard;
//     }

//     // İki CardsCollection'ı birleştirmek için overload edilmiş operatör
//     public static CardCollection operator +(CardCollection first, CardCollection second)
//     {
//         var result = new CardCollection(first.Cards);
//         result.Cards.AddRange(second.Cards);
//         return result;
//     }

//     // Kartları azalan sırayla sıralamak için bir metot
//     public void SortDesc()
//     {
//         Cards?.Sort((x, y) => y.CompareTo(x));
//     }

//     // Kartları artan sırayla sıralamak için bir metot
//     public void SortAsc()
//     {
//         Cards?.Sort();
//     }

//     // CardsCollection'ı temsil eden bir string döndürmek için override edilmiş ToString metodu
//     override public string ToString()
//     {
//         return string.Join(", ", Cards.Select(card => $"{card.GetShortSign()}{card.GetShortValue()}"));
//     }
// }