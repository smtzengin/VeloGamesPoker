using Firebase.Extensions;
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

    public event Action<string, int> OnDataChanged;

    #region Methods

    private async void Start()
    {
        await SetPlayerData();
    }

    //Adding a new user to the database with userId.
    public void RegisterNewUser(string userID, string username)
    {
        User user = new User(userID, username, 2000, 1, 0, 0);
        string json = JsonUtility.ToJson(user);

        FirebaseManager.Instance.UserReference.Child(userID).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to save user data: " + task.Exception);
                }
                else if (task.IsCanceled)
                {
                    Debug.LogError("Canceled to save user data: " + task.Exception);
                }
                else if (task.IsCompleted)
                {

                    Debug.Log("User data saved successfully");
                }
            });
    }

    //Updating score on the database.
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
    //Updating exp on the database.
    public async Task UpdateExp(int value)
    {
        await SetPlayerData();
        playerData["Exp"] = value;
        FirebaseManager.Instance.ChangeUserData("Exp", value);
        OnDataChanged?.Invoke("Exp", value);

        int newLevel = value / 100;
        if (newLevel > playerData["Level"])
        {
            await UpdateLevel(newLevel);
        }
    }
    //Updating Level on the database.
    public async Task UpdateLevel(int value)
    {
        await SetPlayerData();
        playerData["Level"] = value;
        FirebaseManager.Instance.ChangeUserData("Level", value);
        OnDataChanged?.Invoke("Level", value);
    }
    //Updating Chip on the database.
    public async Task UpdateChip(int value)
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
