using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;  
    [SerializeField] private GameObject _losePanel;  
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.InOutQuad; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowPanel(_winPanel);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowPanel(_losePanel);
        }
    }

    private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;  
        panel.transform.DOScale(1, animationDuration).SetEase(animationEase); 
    }

    public void HidePanel(GameObject panel)
    {
        panel.transform.DOScale(0, animationDuration).SetEase(animationEase).OnComplete(() => 
        {
            panel.SetActive(false);
        });
    }
}
