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
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance => Singleton<FirebaseManager>.Instance;
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
        Debug.Log("Auth Başarılı!");
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
            Debug.Log("User Reference : " + _userReference);
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
                RegisterNewUser(result.User.UserId, username);
                ViewManager.Show<LoginView>();
            }
        }
        
    }

    private void RegisterNewUser(string userID, string username)
    {
        User user = new User(userID, username, 2000,1,0,0);
        string json = JsonUtility.ToJson(user);

        _userReference.Child(userID).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to save user data: " + task.Exception);
                }
                else if (task.IsCanceled)
                {
                    Debug.LogError("Canceled to save user data: " + task.Exception);
                }
                else if (task.IsCompleted)
                {

                    Debug.Log("User data saved successfully");
                }
            });
    }

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
            SceneManager.LoadScene("MainMenuScene");
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
            Debug.Log("giriş yapılamadı.");
        }
    }

    private void AutoLogin()
    {
        if (user != null)
        {
            Debug.Log("giriş yapıldı");
            SceneManager.LoadScene("MainMenuScene");
            ViewManager.Show<MainMenuView>();
        }

        else
        {
            Debug.Log("giriş yapılamadı");
        }
    }

    public void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                UnityEngine.Debug.Log("Signed out : " + user.UserId);
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

    public IEnumerator GetUserAllData()
    {
        var task = _userReference.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            string userIDSnapshot = snapshot.Child(user.UserId).Child("UserID").Value.ToString();
            string userNameSnapshot = snapshot.Child(user.UserId).Child("Username").Value.ToString();
            int coinSnapshot = int.Parse(snapshot.Child(user.UserId).Child("Chip").Value.ToString());
            int expSnapshot = int.Parse(snapshot.Child(user.UserId).Child("Exp").Value.ToString());
            int levelSnapshot = int.Parse(snapshot.Child(user.UserId).Child("Level").Value.ToString());
            int scoreSnapshot = int.Parse(snapshot.Child(user.UserId).Child("Score").Value.ToString());

            Debug.LogError(userIDSnapshot + userNameSnapshot + coinSnapshot + expSnapshot);
        }
    }

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

    public async void SetPlayerStats()
    {
        string username = await GetUserStringData("Username");
        string chipCount = (await GetUserIntData("Chip")).ToString();
        string level = (await GetUserIntData("Level")).ToString();
        Debug.Log($"Username : {username}, chipCount : {chipCount}, Level : {level}");
    }

    public void ChangeUserData(string requestedData,int value)
    {
        Dictionary<string, object> childUpdates = new()
        {
            [requestedData] = value,
        };
        _userReference.Child(user.UserId).UpdateChildrenAsync(childUpdates);
    }

}
