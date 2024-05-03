using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RegisterView : View
{
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TextMeshProUGUI _warningText;
    [SerializeField] private Button _registerButton, _backButton;

    private FirebaseManager _firebaseManager;

    public override void Initialize()
    {
        _registerButton.onClick.AddListener(Register);
        _backButton.onClick.AddListener(() => ViewManager.ShowLast());
    }

    private void Awake()
    {
        _usernameInputField.text = string.Empty;
        _emailInputField.text = string.Empty;
        _passwordInputField.text = string.Empty;
        _warningText.text = string.Empty;

        _firebaseManager = FindObjectOfType<FirebaseManager>();
    }

    private void Register()
    {
        if(_usernameInputField != null && _emailInputField != null && _passwordInputField != null)
        {
            string username = _usernameInputField.text;
            string email = _emailInputField.text;
            string password = _passwordInputField.text;

            StartCoroutine(_firebaseManager.Register(email, password, username, _warningText));
        }
        else
        {
            _warningText.text = "Username, email or password is empty!";
        }

        _usernameInputField.text = string.Empty;
        _emailInputField.text = string.Empty;
        _passwordInputField.text = string.Empty;
    }

}
