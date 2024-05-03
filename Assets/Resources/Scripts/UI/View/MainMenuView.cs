using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField] private Button _settingsButton, _playButton;
    [SerializeField] private TextMeshProUGUI _usernameText;
    [SerializeField] private TextMeshProUGUI _userCoinText;
    [SerializeField] private TextMeshProUGUI _userLeveltext;

    private FirebaseManager _firebaseManager;
    private LoadingView _loadingPanel;

    public override void Initialize()
    {
        _settingsButton.onClick.AddListener(() => ViewManager.Show<SettingsView>());
    }

    private void Awake()
    {
        _firebaseManager = FindObjectOfType<FirebaseManager>(true);
        _loadingPanel = FindObjectOfType<LoadingView>(true);
    }

    private void Start()
    {
        SetPlayerStats();
    }

    private async void SetPlayerStats()
    {
        _usernameText.text = await _firebaseManager.GetUserStringData("UserName");
        _userCoinText.text = (await _firebaseManager.GetUserIntData("Coin")).ToString();
        _userLeveltext.text = (await _firebaseManager.GetUserIntData("Level")).ToString();
        _loadingPanel.gameObject.SetActive(false);
    }
}
