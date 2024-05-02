using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Player[] _playerScripts;
    
    private void Awake() =>
        Instance = this;

    private void Start()
    {
        _playerScripts = GameLoopManager.Instance.GetPlayersInLine();
        CreateNewPlayer();
    }

    public void CreateNewPlayer()
    {
        for (int i = 0; i < _playerScripts.Length; i++)
        {
            if (_playerScripts[i].Character != null)
                continue;
            _playerScripts[i].SetCharacter(CharacterSetup.Instance.CreateCharacter(_playerScripts[i], i));
            if (_playerScripts[i].IsLocalPlayer) //Bizim karakter
                ActionHelpers.Instance.SetButtonsPlayer(_playerScripts[i]);

            break;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 0.5f;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            Time.timeScale = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            Time.timeScale = 5;
    }
    public void SendBids(Player p)
    {
        ChipsHandler.Instance.BidChips(p);
    }

}
