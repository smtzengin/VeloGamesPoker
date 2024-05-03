using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : View
{
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private Button _loginButton, _registerButton;
    [SerializeField] private TextMeshProUGUI _warningText;
    private FirebaseManager _firebaseManager;

    public override void Initialize()
    {
        _loginButton.onClick.AddListener(Login);
        _registerButton.onClick.AddListener(() => { ViewManager.Show<RegisterView>(); });
    }

    private void Awake()
    {
        _emailInputField.text = string.Empty;
        _passwordInputField.text = string.Empty;
        _warningText.text = string.Empty;
        _firebaseManager = FindObjectOfType<FirebaseManager>();

    }

    private void Login()
    {        

        if(_emailInputField != null && _passwordInputField != null)
        {
            string email = _emailInputField.text;
            string password = _passwordInputField.text;
            StartCoroutine(_firebaseManager.Login(email, password, _warningText));
        }
        _emailInputField.text = string.Empty;
        _passwordInputField.text = string.Empty;
    }
}
