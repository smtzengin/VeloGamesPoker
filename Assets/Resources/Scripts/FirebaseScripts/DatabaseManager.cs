using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{

    public static DatabaseManager Instance;

    private Dictionary<string, int> playerData = new Dictionary<string, int>
    {
        { "Score", 0 },
        { "Exp", 0 },
        { "Level", 1 },
        { "Coin", 0 }
    };

    private Dictionary<string, TextMeshProUGUI> textElements = new Dictionary<string, TextMeshProUGUI>();

    public event Action<string, int> OnDataChanged;

    private FirebaseManager _firebaseManager;
    private LoadingPanel _loadingPanel;

    #region Methods

    private void Awake()
    {
        Instance = this;
        _firebaseManager = FindObjectOfType<FirebaseManager>(true);
        _loadingPanel = FindObjectOfType<LoadingPanel>(true);
        _loadingPanel.gameObject.SetActive(true);

    }

    private void Start()
    {
        OnDataChanged += HandleDataChanged;
    }

    private void HandleDataChanged(string dataType, int newValue)
    {
        if (textElements.ContainsKey(dataType))
        {
            textElements[dataType].text = newValue.ToString();
        }
    }

    public async void UpdateScore(int value)
    {
        int currentScore = playerData["Score"];
        currentScore += value;
        if (currentScore >= 0)
        {
            playerData["Score"] = currentScore;
            _firebaseManager.ChangeUserData("Score", currentScore);
            OnDataChanged?.Invoke("Score", currentScore);
        }
    }

    public async void UpdateExp(int value)
    {
        int currentExp = playerData["Exp"];
        currentExp += value;
        if (currentExp >= 0)
        {
            playerData["Exp"] = currentExp;
            _firebaseManager.ChangeUserData("Exp", currentExp);
            OnDataChanged?.Invoke("Exp", currentExp);

            int newLevel = currentExp / 100;
            if (newLevel > playerData["Level"])
            {
                UpdateLevel(newLevel);
            }
        }
    }

    public void UpdateLevel(int value)
    {
        playerData["Level"] = value;
        _firebaseManager.ChangeUserData("Level", value);
        OnDataChanged?.Invoke("Level", value);
    }

    public async void UpdateCoin(int value)
    {
        int currentCoin = playerData["Coin"];
        currentCoin += value;
        if (currentCoin >= 0)
        {
            playerData["Coin"] = currentCoin;
            _firebaseManager.ChangeUserData("Coin", currentCoin);
            OnDataChanged?.Invoke("Coin", currentCoin);
        }
    }

    public void Logout()
    {
        if (_firebaseManager.auth != null)
        {
            _firebaseManager.Logout(_firebaseManager.auth);
            gameObject.SetActive(false);
            MainCanvas.instance.LoginPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Auth is null!");
        }
    }

    #endregion
}
