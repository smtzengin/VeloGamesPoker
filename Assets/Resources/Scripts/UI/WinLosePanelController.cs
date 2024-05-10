using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;

public class WinLosePanelController : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private ParticleSystem _winParticles;
    [SerializeField] private Button _winCloseButton, _loseCloseButton, _winContButton, _loseContButton;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Ease _animationEase = Ease.InOutQuad;
    

    void Start()
    {
        // Başlangıçta her iki paneli de pasif yap
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);
        _winParticles.gameObject.SetActive(false);

        // Close butonları için listener ekle
        _winCloseButton.onClick.AddListener(() => { MainMenu(); CloseWinPanel(); });
        _loseCloseButton.onClick.AddListener(() => { MainMenu(); HidePanel(_losePanel); });

        _winContButton.onClick.AddListener(() => { ResetGame(); CloseWinPanel(); });
        _loseContButton.onClick.AddListener(() => { ResetGame(); HidePanel(_losePanel); });
    }

    public void TogglePanel(bool won)
    {
        if (won)
        {
            ShowPanel(_winPanel);
            HidePanel(_losePanel);
            _winParticles.gameObject.SetActive(true);
            _winParticles.Play();
        }
        else
        {
            ShowPanel(_losePanel);
            HidePanel(_winPanel);
            _winParticles.Stop();
            _winParticles.gameObject.SetActive(false);
        }
    }

    private void ShowPanel(GameObject panel, bool animate = true)
    {
        panel.SetActive(true);
        if (animate)
        {
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(1, _animationDuration).SetEase(_animationEase);      
        }
    }

    private void HidePanel(GameObject panel)
    {
        panel.transform.DOScale(0, _animationDuration).SetEase(_animationEase).OnComplete(() => panel.SetActive(false));
    }

    private void CloseWinPanel()
    {
        HidePanel(_winPanel);
        _winParticles.Stop();
        _winParticles.gameObject.SetActive(false);
    }
    private void ResetGame()
    {
        UIManager.ResetGame();
    }
    private void MainMenu()
    {
        StartCoroutine(LoadingCanvas.Instance.LoadNewScene("MainMenuScene"));
    }

    
}
