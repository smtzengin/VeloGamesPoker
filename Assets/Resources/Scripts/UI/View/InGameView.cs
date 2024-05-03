using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : View
{
    [SerializeField] private Button _settingsButton;

    public override void Initialize()
    {
        _settingsButton.onClick.AddListener(() => ViewManager.Show<SettingsView>());
    }
}
