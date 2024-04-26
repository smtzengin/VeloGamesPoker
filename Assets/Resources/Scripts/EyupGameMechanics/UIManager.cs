using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    [SerializeField] private Image[] _handCards;
    [SerializeField] private Image[] _tableCards;
    private byte _handIndex = 0;
    private byte _tableIndex = 0;
    [SerializeField] private Button[] _buttons;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < _handCards.Length; i++)
        {
            _handCards[i].enabled = false;
        }
    }
    public static void AddCard(CardSO card, bool toHand)
    {
        if (toHand)
            instance.AddCardToHand(card);
        else
            instance.AddCardToTable(card);
    }
    private void AddCardToHand(CardSO card)
    {
        _handCards[_handIndex % _handCards.Length].sprite = card.CardSprite;
        _handCards[_handIndex++ % _handCards.Length].enabled = true;
    }
    private void AddCardToTable(CardSO card)
    {
        _tableCards[_tableIndex % _tableCards.Length].sprite = card.CardSprite;
        _tableCards[_tableIndex++ % _tableCards.Length].enabled = true;
    }

    public static void ButtonActive(bool active)
    {
        instance.SetButton(active);
    }
    private void SetButton(bool active)
    {
        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].interactable = active;
    }

}
