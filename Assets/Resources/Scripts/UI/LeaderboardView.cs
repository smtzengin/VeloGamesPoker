using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Firebase.Database;
using System;
using System.Linq;
using TMPro;
using System.Threading.Tasks;

public class LeaderboardView : View
{
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private float _animationDuration = .2f;
    [SerializeField] private Ease _animationEase = Ease.InOutQuad;

    [SerializeField] private List<GameObject> leaderboardGameObjects;

    void OnEnable()
    {
        GetLeaderboardData();
    }
    private void OnDisable()
    {
        HidePanel();
    }
    public void ShowPanel()
    {
        _leaderboardPanel.transform.localScale = Vector3.zero;
        _leaderboardPanel.transform.DOScale(1, _animationDuration).SetEase(_animationEase);
        ListOfLeaderboardGameObjects();
    }

    private void HidePanel()
    {
        _leaderboardPanel.transform.DOScale(0, _animationDuration).SetEase(_animationEase).OnComplete(() => _leaderboardPanel.SetActive(false));
    }

    public override void Initialize()
    {
        HidePanel();
        _closeButton.onClick.AddListener(() => ViewManager.ShowLast());
    }

    public void ListOfLeaderboardGameObjects()
    {

        for (int i = 0; i < leaderboardGameObjects.Count; i++)
        {
            int newScale = i * 120;
            leaderboardGameObjects[i].transform.DOLocalMoveY(250 - newScale, 0.5f);
        }
    }

    public async void GetLeaderboardData()
    {
        try
        {
            List<LeaderboardUserData> leaderboardData = await FetchLeaderboardData();
            LeaderboardUserData userPlayerData = await FetchUserPlayerData();

            UpdateLeaderboardUI(leaderboardData, userPlayerData);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to retrieve data: " + ex.Message);
        }
    }

    private async Task<List<LeaderboardUserData>> FetchLeaderboardData()
    {
        DataSnapshot snapshot = await FirebaseManager.Instance.UserReference.OrderByChild("Chip").LimitToLast(5).GetValueAsync();

        List<LeaderboardUserData> leaderboardData = new List<LeaderboardUserData>();

        foreach (var childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
        {
            string username = childSnapshot.Child("Username").Value.ToString();
            int userChipCount = int.Parse(childSnapshot.Child("Chip").Value.ToString());

            leaderboardData.Add(new LeaderboardUserData(username, userChipCount));
        }

        return leaderboardData;
    }

    private async Task<LeaderboardUserData> FetchUserPlayerData()
    {
        LeaderboardUserData userPlayerData = null;
        if (!string.IsNullOrEmpty(FirebaseManager.Instance.auth.CurrentUser.UserId))
        {
            DataSnapshot userSnapshot = await FirebaseManager.Instance.UserReference.Child(FirebaseManager.Instance.auth.CurrentUser.UserId).GetValueAsync();

            string username = userSnapshot.Child("Username").Value.ToString();
            int userChipCount = int.Parse(userSnapshot.Child("Chip").Value.ToString());

            userPlayerData = new LeaderboardUserData(username, userChipCount);
        }

        return userPlayerData;
    }

    private void UpdateLeaderboardUI(List<LeaderboardUserData> leaderboardData, LeaderboardUserData userPlayerData)
    {
        for (int i = 0; i < leaderboardData.Count; i++)
        {
            leaderboardGameObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = leaderboardData[i].Username + "-" +leaderboardData[i].ChipCount;
        }

        if (userPlayerData != null)
        {
            leaderboardGameObjects[5].GetComponentInChildren<TextMeshProUGUI>().text = userPlayerData.Username + "-" + userPlayerData.ChipCount;
        }

        ShowPanel();

        for (int j = 0; j < leaderboardData.Count; j++)
        {
            Debug.Log($" Sıralama : {j + 1}  Player Username : {leaderboardData[j].Username} ");
        }
    }

}
