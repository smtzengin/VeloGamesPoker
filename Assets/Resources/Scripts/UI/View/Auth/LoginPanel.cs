using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private TextMeshProUGUI warningText;
    private FirebaseManager _firebaseManager;

    private void Awake()
    {
        emailInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
        warningText.text = string.Empty;
        _firebaseManager = FindObjectOfType<FirebaseManager>();
        loginButton.onClick.AddListener(Login);
    }

    private void Login()
    {        

        if(emailInputField != null && passwordInputField != null)
        {
            string email = emailInputField.text;
            string password = passwordInputField.text;
            StartCoroutine(_firebaseManager.Login(email, password, warningText));
        }


        emailInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
    }
}
