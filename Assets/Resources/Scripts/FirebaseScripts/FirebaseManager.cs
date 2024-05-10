using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using Firebase.Database;
using System.Collections;
using TMPro;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Resources.Scripts.Utility;


public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance => Singleton<FirebaseManager>.Instance;

    [Header("Firebase Properties")]
    public FirebaseAuth auth;
    public FirebaseUser user;
    private DatabaseReference _userReference;
    private DependencyStatus _dependencyStatus = DependencyStatus.UnavailableOther;
    public DatabaseReference UserReference { get => _userReference; private set => _userReference = value; }


    private void Awake()
    {
        StartCoroutine(CheckAndFixDependenciesAsync());
    }
    private void InitiliazeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckAndFixDependenciesAsync()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        _dependencyStatus = dependencyTask.Result;

        if (_dependencyStatus == DependencyStatus.Available)
        {
            InitiliazeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
            _userReference = FirebaseDatabase.DefaultInstance.GetReference("users");
            ViewManager.Show<LoginView>();
        }
        else
        {
            Debug.LogError("Could not resolve all Firebase dependencies " + _dependencyStatus);
        }

    }

    private void OnDestroy()
    {
        if (auth != null) auth = null;
    }

    //Register Function
    public IEnumerator Register(string email,string password,string username,TextMeshProUGUI warningText)
    {
        string output;
        TextMeshProUGUI wMessage = warningText;        
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (email == "") wMessage.text = "Please Enter A Email";
        else if (password == "") wMessage.text = "Please Enter A Password";
        else
        {
            if (registerTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                output = "Unkown error, Please Try Again..";
                switch (error)
                {
                    case AuthError.MissingEmail:
                        output = "Please Enter Your Email";
                        break;
                    case AuthError.MissingPassword:
                        output = "Please Enter Your Password";
                        break;
                    case AuthError.WeakPassword:
                        output = "Weak Password!";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        output = "Email Already In Use";
                        break;
                }
                output = wMessage.text;
            }
            else if (registerTask.IsCompleted)
            {
                AuthResult result = registerTask.Result;
                Debug.LogFormat("Firebase user created successfully : {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
                DatabaseManager.Instance.RegisterNewUser(result.User.UserId, username);
                ViewManager.Show<LoginView>();
            }
        }
        
    }


    //Login Function
    public IEnumerator Login(string email, string password, TextMeshProUGUI warningText)
    {
        TextMeshProUGUI wMessage = warningText;

        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown error, Please Try Again..";
            switch (error)
            {
                case AuthError.MissingEmail:
                    output = "Please Enter Your Email";
                    break;
                case AuthError.MissingPassword:
                    output = "Please Enter Your Password";
                    break;
                case AuthError.UserNotFound:
                    output = "Account Does Not Exist";
                    break;
            }
            if (wMessage.text != null)
            {
                wMessage.text = output;
            }
            else
            {
                Debug.LogError("loginErrorText is null!");
            }
           
        }
        else if (loginTask.IsCompleted)
        {
            AuthResult result = loginTask.Result;
            Debug.LogFormat("User signed in succesfully : {0} {1}", result.User.DisplayName, result.User.UserId);
            StartCoroutine(LoadingCanvas.Instance.LoadNewScene("MainMenuScene"));
            ViewManager.Show<MainMenuView>();
        }
    }

    private IEnumerator CheckForAutoLogin()
    {
        if (user != null)
        {
            var reloadUserTask = user.ReloadAsync();
            yield return new WaitUntil(() => reloadUserTask.IsCompleted);
            yield return new WaitForSeconds(.5f);
            AutoLogin();
        }
        else
        {
            //if the user cannot log in.
        }
    }

    private void AutoLogin()
    {
        if (user != null)
        {
            StartCoroutine(LoadingCanvas.Instance.LoadNewScene("MainMenuScene"));
            ViewManager.Show<MainMenuView>();
        }

        else
        {
            //if the user cannot log in.
        }
    }

    public void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out : " + user.UserId);
            }
            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed In : " + user.UserId);
            }

        }
    }

    public void Logout()
    {
        if(auth.CurrentUser != null)
        {
            Debug.Log($"Logout User : {auth.CurrentUser.UserId}");
            auth.StateChanged -= AuthStateChanged;
            auth.SignOut();
        }
        else
        {
            Debug.Log($"Current User ID is null! : {auth.CurrentUser.UserId}");
        }
    }

    //Get user string data
    public async Task<string> GetUserStringData(string requestedData)
    {
        try
        {
            DataSnapshot snapshot = await _userReference.Child(user.UserId).Child(requestedData).GetValueAsync();
            return snapshot.Value.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log($"Error getting user data: {ex.Message}");
            return null;
        }
    }

    //Get user int data
    public async Task<int> GetUserIntData(string requestedData)
    {
        try
        {
            DataSnapshot snapshot = await _userReference.Child(user.UserId).Child(requestedData).GetValueAsync();
            return int.Parse(snapshot.Value.ToString());
        }
        catch (Exception ex)
        {
            Debug.Log($"Error getting user data: {ex.Message}");
            return 0;
        }
    }

    //Change user data
    public void ChangeUserData(string requestedData,int value)
    {
        Dictionary<string, object> childUpdates = new()
        {
            [requestedData] = value,
        };
        _userReference.Child(user.UserId).UpdateChildrenAsync(childUpdates);
    }

}
