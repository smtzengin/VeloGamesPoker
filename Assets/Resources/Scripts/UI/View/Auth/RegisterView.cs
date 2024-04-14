using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterView : View
{
    [SerializeField] private Button _registerButton, _backButton;
    [SerializeField] private TMP_InputField _mail, _username, _password;
    public override void Initialize()
    {
        _backButton.onClick.AddListener(() => ViewManager.ShowLast());
        _registerButton.onClick.AddListener(Register);
    }

    private void Register()
    {
        // Get information from _mail _username and _password and register the user.
    }
}
