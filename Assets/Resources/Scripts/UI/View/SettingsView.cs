using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : View
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _leftButton, _rightButton;
    [SerializeField] private TextMeshProUGUI _musicText;

    public override void Initialize()
    {
        _backButton.onClick.AddListener(() => ViewManager.ShowLast());
        _leftButton.onClick.AddListener(() => { ChangeMusic(false);});
        _rightButton.onClick.AddListener(() => { ChangeMusic(true);});
    }
    private void Start()
    {
        DisplayMusicName();
    }

    public void DisplayMusicName()
    {
        _musicText.text = BGM.Instance.GetMusicName();
    }

    public void ChangeMusic(bool right)
    {
        BGM.Instance.ChangeSong(right);
        DisplayMusicName();
    }
}
