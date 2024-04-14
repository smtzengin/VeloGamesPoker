using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : View
{
    [SerializeField] private Button _loginButton, _registerButton;
    [SerializeField] private TMP_InputField _username, _password;
    public override void Initialize()
    {
        _loginButton.onClick.AddListener(LoginButton);
        _registerButton.onClick.AddListener(() => ViewManager.Show<RegisterView>());
    }
    private void LoginButton()
    {
        //Get information from _username and _password and login.
    }
}
