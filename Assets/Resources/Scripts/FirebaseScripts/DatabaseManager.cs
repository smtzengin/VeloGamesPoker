using Resources.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance => Singleton<DatabaseManager>.Instance;

    private static int score;
    private static int exp;
    private static int level;
    private static int chip;

    private Dictionary<string, int> playerData = new Dictionary<string, int>
    {
        { "Score", score},
        { "Exp", exp },
        { "Level", level },
        { "Chip", chip }
    };

    private Dictionary<string, TextMeshProUGUI> textElements = new Dictionary<string, TextMeshProUGUI>();

    public event Action<string, int> OnDataChanged;

    #region Methods

    private async void Start()
    {
        OnDataChanged += HandleDataChanged;
        await SetPlayerData();
        Debug.Log($"Current Chip : {playerData["Chip"]}");
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
        await SetPlayerData();
        int currentScore = playerData["Score"];
        currentScore += value;
        if (currentScore >= 0)
        {
            playerData["Score"] = currentScore;
            FirebaseManager.Instance.ChangeUserData("Score", currentScore);
            OnDataChanged?.Invoke("Score", currentScore);
        }
    }

    public async void UpdateExp(int value)
    {
        await SetPlayerData();
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
                await UpdateLevel(newLevel);
            }
        }
    }

    public async Task UpdateLevel(int value)
    {
        await SetPlayerData();
        playerData["Level"] = value;
        FirebaseManager.Instance.ChangeUserData("Level", value);
        OnDataChanged?.Invoke("Level", value);
    }
    public async void UpdateChip(int value)
    {
        await SetPlayerData();
        int currentChip = playerData["Chip"];
        currentChip += value;
        playerData["Chip"] = currentChip;
        FirebaseManager.Instance.ChangeUserData("Chip", currentChip);
        OnDataChanged?.Invoke("Chip", currentChip);
    }

    public async Task SetPlayerData()
    {
        score = await FirebaseManager.Instance.GetUserIntData("Score");
        exp = await FirebaseManager.Instance.GetUserIntData("Exp");
        level = await FirebaseManager.Instance.GetUserIntData("Level");
        chip = await FirebaseManager.Instance.GetUserIntData("Chip");

        playerData["Score"] = score;
        playerData["Exp"] = exp;
        playerData["Level"] = level;
        playerData["Chip"] = chip;
    }
    #endregion
}
