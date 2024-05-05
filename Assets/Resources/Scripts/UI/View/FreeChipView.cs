using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreeChipView : MonoBehaviour
{
    [SerializeField] private PlayerInfoView _playerInfoView;

    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private TextMeshProUGUI _infoText;

    private int _randomChipCount;


    private void Awake()
    {
        _randomChipCount = Random.Range(1000, 2000);
        _infoText.text = $"It seems like your chips have decreased. Would you like {_randomChipCount} chips?";
        _yesButton.onClick.AddListener(() => YesButton());
        _noButton.onClick.AddListener(() => NoButton());
    }
    public async void YesButton()
    {
         await DatabaseManager.Instance.UpdateChip(_randomChipCount);
        _playerInfoView.SetPlayerStats();
        gameObject.SetActive(false);
    }

    public async void NoButton()
    {
        gameObject.SetActive(false);
    }

}
