using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _usernameText;
    [SerializeField] private TextMeshProUGUI _userChipCount;
    [SerializeField] private TextMeshProUGUI _userLevelText;
    [SerializeField] private TextMeshProUGUI _userCurrentExpText;
    [SerializeField] private TextMeshProUGUI _userScoreText;

    [SerializeField] private FirebaseManager _firebaseManager;

    public event Action<string, int> OnPlayerInfoDataChanged;

    private void Awake()
    {
        _firebaseManager = FirebaseManager.Instance;

    }
    private void Start()
    {
        SetPlayerStats();

        OnPlayerInfoDataChanged += HandleDataChanged;
    }
    public async void SetPlayerStats()
    {
        _usernameText.text = await _firebaseManager.GetUserStringData("Username");
        _userLevelText.text = (await _firebaseManager.GetUserIntData("Level")).ToString();
        _userCurrentExpText.text = (await _firebaseManager.GetUserIntData("Exp")).ToString();
        _userScoreText.text = (await _firebaseManager.GetUserIntData("Score")).ToString();
        OnPlayerInfoDataChanged.Invoke("Chip", (await _firebaseManager.GetUserIntData("Chip")));
    }

    private void HandleDataChanged(string dataType, int newValue)
    {
        switch (dataType)
        {
            case "Chip":
                StartCoroutine(AnimateValueChange(_userChipCount, int.Parse(_userChipCount.text), newValue));
                break;
        }
    }
    private IEnumerator AnimateValueChange(TextMeshProUGUI text, int startValue, int endValue)
    {
        float duration = .2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));
            text.text = currentValue.ToString();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.text = endValue.ToString();
    }

}
