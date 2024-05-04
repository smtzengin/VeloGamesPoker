using Resources.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance => Singleton<DatabaseManager>.Instance;

    private Dictionary<string, int> playerData = new Dictionary<string, int>
    {
        { "Score", 0 },
        { "Exp", 0 },
        { "Level", 1 },
        { "Chip", 0 }
    };

    private Dictionary<string, TextMeshProUGUI> textElements = new Dictionary<string, TextMeshProUGUI>();

    public event Action<string, int> OnDataChanged;

    #region Methods

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

    public void UpdateScore(int value)
    {
        int currentScore = playerData["Score"];
        currentScore += value;
        if (currentScore >= 0)
        {
            playerData["Score"] = currentScore;
            FirebaseManager.Instance.ChangeUserData("Score", currentScore);
            OnDataChanged?.Invoke("Score", currentScore);
        }
    }

    public void UpdateExp(int value)
    {
        int currentExp = playerData["Exp"];
        currentExp += value;
        if (currentExp >= 0)
        {
            playerData["Exp"] = currentExp;
            FirebaseManager.Instance.ChangeUserData("Exp", currentExp);
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
        FirebaseManager.Instance.ChangeUserData("Level", value);
        OnDataChanged?.Invoke("Level", value);
    }

    public void UpdateCash(int value)
    {
        int currentCoin = playerData["Chip"];
        currentCoin += value;
        if (currentCoin >= 0)
        {
            playerData["Chip"] = currentCoin;
            FirebaseManager.Instance.ChangeUserData("Chip", currentCoin);
            OnDataChanged?.Invoke("Chip", currentCoin);
        }
    }
    #endregion
}
