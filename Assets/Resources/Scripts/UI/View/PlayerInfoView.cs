using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _usernameText;
    [SerializeField] private TextMeshProUGUI _userChipCount;
    [SerializeField] private TextMeshProUGUI _userLeveltext;

    [SerializeField] private FirebaseManager _firebaseManager;

    private void Awake()
    {
        _firebaseManager = FirebaseManager.Instance;

    }
    private void Start()
    {
        SetPlayerStats();
    }
    private async void SetPlayerStats()
    {
        Debug.Log("Auth : " + _firebaseManager.user.UserId);
        _usernameText.text = await _firebaseManager.GetUserStringData("Username");
        _userChipCount.text = (await _firebaseManager.GetUserIntData("Chip")).ToString();
        _userLeveltext.text = (await _firebaseManager.GetUserIntData("Level")).ToString();

    }

}
