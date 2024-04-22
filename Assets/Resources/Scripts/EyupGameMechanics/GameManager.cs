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
            if (i == 1) //Bizim karakter
                ActionHelpers.Instance.SetButtonsPlayer(_playerScripts[i]);

            break;
        }
    }
    public void SendBids(Player p)
    {
        GameObject chips = ChipsHandler.Instance.BidChips(p);
    }
    //public void CheckAllPlayer()
    //{
    //    for (int i = 0; i < _playerScripts.Length; i++)
    //        if (!_playerScripts[i].IsFull)
    //            return;

    //    //StartGame
    //}

}
