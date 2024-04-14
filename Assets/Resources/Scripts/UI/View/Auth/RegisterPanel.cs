using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RegisterPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Button registerButton;

    private FirebaseManager _firebaseManager;

    private void Awake()
    {
        usernameInputField.text = string.Empty;
        emailInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
        warningText.text = string.Empty;

        _firebaseManager = FindObjectOfType<FirebaseManager>();

        registerButton.onClick.AddListener(Register);
    }

    private void Register()
    {
        if(usernameInputField != null && emailInputField != null && passwordInputField != null)
        {
            string username = usernameInputField.text;
            string email = emailInputField.text;
            string password = passwordInputField.text;

            StartCoroutine(_firebaseManager.Register(email, password, username, warningText));
        }
        else
        {
            warningText.text = "Username, email or password is empty!";
        }

        usernameInputField.text = string.Empty;
        emailInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
    }

}
