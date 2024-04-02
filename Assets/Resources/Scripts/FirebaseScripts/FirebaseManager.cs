using Google;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using Firebase.Database;
using System.Collections;
using Unity.VisualScripting;

public class FirebaseManager : MonoBehaviour
{
    private Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;
    private DatabaseReference userReference;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;


    private void Awake()
    {
        StartCoroutine(CheckAndFixDependenciesAsync());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Register("deneme@gmail.com", "deneme"));
        }
    }

    private void InitiliazeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
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
            userReference = FirebaseDatabase.DefaultInstance.GetReference("users");
        }
        else
        {
            UnityEngine.Debug.LogError("Could not resolve all Firebase dependencies " + dependencyStatus);
        }

    }

    private void OnDestroy()
    {
        if (auth != null) auth = null;
    }

    private IEnumerator Register(string email,string password)
    {
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if(registerTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            Debug.LogError(error);
        }
        else if (registerTask.IsCompleted)
        {
            AuthResult result = registerTask.Result;
            Debug.LogFormat("Firebase user created successfully : {0} ({1})",
                result.User.DisplayName,result.User.UserId);
            RegisterNewUser(result.User.UserId);            
        }
    }

    private void RegisterNewUser(string userID)
    {
        User user = new User(userID, "deneme", 0, 0, 0, 0);
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
}
