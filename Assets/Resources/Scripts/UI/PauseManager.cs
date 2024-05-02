using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel; 
    [SerializeField] private Button _continueButton; 

    private void Awake()
    {
        _pausePanel.SetActive(false);
    }
    void Start()
    {
        _continueButton.onClick.AddListener(ContinueGame);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Time.timeScale = 0;
            _pausePanel.SetActive(true);
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        _pausePanel.SetActive(false);
    }
}