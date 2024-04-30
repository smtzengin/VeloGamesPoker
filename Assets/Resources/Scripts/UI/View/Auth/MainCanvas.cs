using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas instance;

    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject DatabaseExamplePanel;
    public GameObject MainMenuPanel;

    private void Awake()
    {
        if (instance == null) instance = this;
    }


}
