using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DatabaseExamplePanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI userNicknameText;
    [SerializeField] private TextMeshProUGUI userExpText;
    [SerializeField] private TextMeshProUGUI userLeveltext;
    [SerializeField] private TextMeshProUGUI userChipText;
    [SerializeField] private TextMeshProUGUI userScoreText;

    [SerializeField] private Button increaseScore;
    [SerializeField] private Button decreaseScore;
    [SerializeField] private Button increaseExp;
    [SerializeField] private Button decreaseExp;
    [SerializeField] private Button increaseCoin;
    [SerializeField] private Button decreaseCoin;

    [SerializeField] private Button logoutButton;

    [SerializeField]private FirebaseManager _firebaseManager;
    private LoadingView _loadingPanel;

    public event Action<string, int> OnDataChanged;

    private void Awake()
    {
        _firebaseManager = FindObjectOfType<FirebaseManager>(true);
        _loadingPanel = FindObjectOfType<LoadingView>(true);
        logoutButton.onClick.AddListener(Logout);

        _loadingPanel.gameObject.SetActive(true);
    }


    private void Start()
    {
        SetPlayerStats();
        OnDataChanged += HandleDataChanged;
    }

    private void HandleDataChanged(string dataType, int newValue)
    {
        switch (dataType)
        {
            case "Score":
                StartCoroutine(AnimateValueChange(userScoreText, int.Parse(userScoreText.text), newValue));
                break;
            case "Exp":
                StartCoroutine(AnimateValueChange(userExpText, int.Parse(userExpText.text), newValue));
                break;
            case "Chip":
                StartCoroutine(AnimateValueChange(userChipText, int.Parse(userChipText.text), newValue));
                break;
            case "Level":
                StartCoroutine(AnimateValueChange(userLeveltext, int.Parse(userLeveltext.text), newValue));
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


    private async void SetPlayerStats()
    {
        userNicknameText.text = await _firebaseManager.GetUserStringData("Username");
        userExpText.text = (await _firebaseManager.GetUserIntData("Exp")).ToString();
        userChipText.text = (await _firebaseManager.GetUserIntData("Chip")).ToString();
        userLeveltext.text = (await _firebaseManager.GetUserIntData("Level")).ToString();
        userScoreText.text = (await _firebaseManager.GetUserIntData("Score")).ToString();

        _loadingPanel.gameObject.SetActive(false);
    }

    public async void UpdateScore(int value)
    {
        int currentScore = await _firebaseManager.GetUserIntData("Score");
        currentScore += value;
        if(currentScore >= 0)
        {
            _firebaseManager.ChangeUserData("Score", currentScore);

            OnDataChanged?.Invoke("Score", currentScore);
        }
    }

    public async void UpdateExp(int value)
    {
        int currentExp = await _firebaseManager.GetUserIntData("Exp");
        currentExp += value;
        if(currentExp >= 0)
        {
            _firebaseManager.ChangeUserData("Exp", currentExp);

            OnDataChanged?.Invoke("Exp", currentExp);

            int newLevel = (currentExp / 100);
            if (newLevel == 0)
            {
                newLevel = 1;
                UpdateLevel(newLevel);
            }
            else
            {
                UpdateLevel(newLevel);
            }
        }
       
    }

    public void UpdateLevel(int value)
    {
        _firebaseManager.ChangeUserData("Level", value);

        OnDataChanged?.Invoke("Level", value);
    }
    public async void UpdateCoin(int value)
    {
        int currentCoin = await _firebaseManager.GetUserIntData("Coin");
        currentCoin += value;
        if(currentCoin >= 0)
        {
            _firebaseManager.ChangeUserData("Coin", currentCoin);

            OnDataChanged?.Invoke("Coin", currentCoin);
        }
    }

    public void Logout()
    {
        if(_firebaseManager.auth != null)
        {
            _firebaseManager.Logout();
        }
        else
        {
            Debug.Log("Auth is null!");
        }
    }

}

