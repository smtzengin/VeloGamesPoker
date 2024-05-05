using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField] private Button _settingsButton, _playButton, _leaderboardButton,_logoutButton;

    [SerializeField] private Button _increaseButton;
    public override void Initialize()
    {
        _settingsButton.onClick.AddListener(() => ViewManager.Show<SettingsView>());
        _logoutButton.onClick.AddListener(() =>{ Logout(); });
        _leaderboardButton.onClick.AddListener(() => { 
            ViewManager.Show<LeaderboardView>();       
        });
        _playButton.onClick.AddListener(() => SceneManager.LoadScene("Eyup"));
        _increaseButton.onClick.AddListener(() => DatabaseManager.Instance.UpdateChip(120)); 
    }

    public void Logout()
    {
        if (FirebaseManager.Instance.auth != null)
        {
            FirebaseManager.Instance.Logout();
            SceneManager.LoadScene("GUI");         
            ViewManager.Show<LoginView>();
        }
        else
            Debug.Log("Auth is null!");
    }

}
