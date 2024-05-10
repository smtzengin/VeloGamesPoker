using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField] private Button _settingsButton, _playButton, _leaderboardButton,_logoutButton;
    [SerializeField] private FreeChipView _freeChipView;

    private async void OnEnable()
    {
        int userChipCount = await FirebaseManager.Instance.GetUserIntData("Chip");
        if(userChipCount < 500)
        {
            _freeChipView.gameObject.SetActive(true);
        }
    }

    public override void Initialize()
    {
        _settingsButton.onClick.AddListener(() => ViewManager.Show<SettingsView>());
        _logoutButton.onClick.AddListener(() =>{ Logout(); });
        _leaderboardButton.onClick.AddListener(() => { 
            ViewManager.Show<LeaderboardView>();       
        });
        _playButton.onClick.AddListener(() => StartCoroutine(LoadingCanvas.Instance.LoadNewScene("GameScene")));
    }

    public void Logout()
    {
        if (FirebaseManager.Instance.auth != null)
        {
            FirebaseManager.Instance.Logout();
            StartCoroutine(LoadingCanvas.Instance.LoadNewScene("GUI"));         
            ViewManager.Show<LoginView>();
        }
        else
            Debug.Log("Auth is null!");
    }


}
