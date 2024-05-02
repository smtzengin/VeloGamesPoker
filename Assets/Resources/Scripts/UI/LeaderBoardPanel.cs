using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LeaderBoardPanel : MonoBehaviour
{
    [SerializeField] private GameObject _leaderboardPanel;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Ease _animationEase = Ease.InOutQuad;

    void Start()
    {
        _leaderboardPanel.SetActive(false);

        _openButton.onClick.AddListener(() => ShowPanel());
        _closeButton.onClick.AddListener(() => HidePanel());
    }

    public void ShowPanel()
    {
        _leaderboardPanel.SetActive(true);
        _leaderboardPanel.transform.localScale = Vector3.zero;
        _leaderboardPanel.transform.DOScale(1, _animationDuration).SetEase(_animationEase);
    }

    private void HidePanel()
    {
        _leaderboardPanel.transform.DOScale(0, _animationDuration).SetEase(_animationEase).OnComplete(() => _leaderboardPanel.SetActive(false));
    }
}
