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

public class FirebaseManager : MonoBehaviour
{


    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;
    private DatabaseReference userReference;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;


    private void Awake()
    {
        StartCoroutine(CheckAndFixDependenciesAsync());
        
       
    }


    private void InitiliazeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        Debug.Log("Auth Başarılı!");
    }

    private IEnumerator CheckAndFixDependenciesAsync()
    {
        var dependencyTask = Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        dependencyStatus = dependencyTask.Result;

        if (dependencyStatus == DependencyStatus.Available)
        {
            InitiliazeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
            userReference = FirebaseDatabase.DefaultInstance.GetReference("users");
            Debug.Log(userReference);
        }
        else
        {
            Debug.LogError("Could not resolve all Firebase dependencies " + dependencyStatus);
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
                MainCanvas.instance.RegisterPanel.SetActive(false);
                MainCanvas.instance.LoginPanel.SetActive(true);
            }
        }
        
    }

    private void RegisterNewUser(string userID, string username)
    {
        User user = new User(userID, username, 0, 0, 0, 0);
        string json = JsonUtility.ToJson(user);

        userReference.Child(userID).SetRawJsonValueAsync(json)
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
            MainCanvas.instance.LoginPanel.SetActive(false);
            MainCanvas.instance.DatabaseExamplePanel.SetActive(true);


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
            MainCanvas.instance.LoginPanel.SetActive(false);
            MainCanvas.instance.DatabaseExamplePanel.SetActive(true);
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

    public void Logout(FirebaseAuth auth)
    {
        auth.StateChanged -= AuthStateChanged;
        auth.SignOut();
        Debug.Log($"Logout User : {auth.CurrentUser.UserId}");
    }

    public IEnumerator GetUserAllData()
    {
        var task = userReference.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            string userIDSnapshot = snapshot.Child(user.UserId).Child("UserID").Value.ToString();
            string userNameSnapshot = snapshot.Child(user.UserId).Child("UserName").Value.ToString();
            int coinSnapshot = int.Parse(snapshot.Child(user.UserId).Child("Coin").Value.ToString());
            int expSnapshot = int.Parse(snapshot.Child(user.UserId).Child("Exp").Value.ToString());
            int levelSnapshot = int.Parse(snapshot.Child(user.UserId).Child("Level").Value.ToString());
            int scoreSnapshot = int.Parse(snapshot.Child(user.UserId).Child("Score").Value.ToString());

        }
    }

    public async Task<string> GetUserStringData(string requestedData)
    {
        try
        {
            DataSnapshot snapshot = await userReference.Child(user.UserId).Child(requestedData).GetValueAsync();
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
            DataSnapshot snapshot = await userReference.Child(user.UserId).Child(requestedData).GetValueAsync();
            return int.Parse(snapshot.Value.ToString());
        }
        catch (Exception ex)
        {
            Debug.Log($"Error getting user data: {ex.Message}");
            return 0;
        }
    }

    public void ChangeUserData(string requestedData,int value)
    {
        Dictionary<string, object> childUpdates = new()
        {
            [requestedData] = value,
        };
        userReference.Child(user.UserId).UpdateChildrenAsync(childUpdates);
    }


}
