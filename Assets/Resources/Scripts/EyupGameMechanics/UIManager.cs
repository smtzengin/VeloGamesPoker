using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    [SerializeField] private Image[] _handCards;
    [SerializeField] private Image[] _tableCards;
    [SerializeField] private Image _levelFill;
    private byte _handIndex = 0;
    private byte _tableIndex = 0;
    [SerializeField] private TextMeshProUGUI _tableChipText, _raiseText, _levelText;
    // 0. FOLD, 1. Call, 2. All In One, 3. Raise, 4. Decrease, 5. Increase
    [SerializeField] private Button[] _buttons;
    [SerializeField] private GameController _controller;
    private void Awake()
    {
        instance = this;
    }

    //Add card to hand or table
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

    //Set all butons to active or not.
    public static void AllButtonsActive(bool active)
    {
        instance.SetButton(active);
    }
    private void SetButton(bool active)
    {
        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].interactable = active;
    }
    //Set one buton to active or not.
    public static void ButtonActive(int line, bool allow)
    {
        instance._buttons[line].interactable = allow;
    }
    //Set call button's text to check or call
    public static void CallCheckText(bool isCheck)
    {
        instance._buttons[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = isCheck ? "Check" : "Call";
    }
    public static void UpdateTableChipText(int amount)
    {
        instance._tableChipText.text = amount.ToString();
    }
    public static void UpdateRaiseChipText(int amount)
    {
        instance._raiseText.text = amount.ToString();
    }
    public static void UpdateChipText(TextMeshProUGUI playerChips, int amount)
    {
        playerChips.text = amount.ToString();
    }
    public static void UpdateLevel(int level, float nextXP, float currentXP)
    {
        instance.UpdateLevelStats(level, nextXP, currentXP);
    }
    private void UpdateLevelStats(int level, float nextXP, float currentXP)
    {
        float fill = Mathf.InverseLerp(0f, nextXP, currentXP);
        _levelFill.fillAmount = fill;
        _levelText.text = level.ToString();
    }

    public static void ReturnToMain()
    {
        SceneManager.LoadScene(1);
    }
    public static void ResetGame()
    {
        instance.Reset();
        GameLoopManager.Instance.ResetGame();
    }
    private void Reset()
    {
        _handIndex = _tableIndex = 0;
        for (int i = 0; i < _handCards.Length; i++)
            _handCards[i].enabled = false;
        for (int i = 0; i < _tableCards.Length; i++)
            _tableCards[i].enabled = false;
    }
    public static void ToggleEndPanel(bool won)
    {
        Debug.Log("EndToggle");
        instance._controller.TogglePanel(won);

    }

}
