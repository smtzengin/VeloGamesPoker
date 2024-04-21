using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] Characters { get; private set; }
    private List<Player> _playerScripts;

    private void Awake() =>
        Instance = this;

    private void Start()
    {
        Characters = new GameObject[4];
        _playerScripts = GameLoopManager.Instance.GetPlayers();
        CreateNewPlayer();
    }

    public void CreateNewPlayer()
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] != null)
                continue;
            Characters[i] = CharacterSetup.Instance.CreateCharacter(_playerScripts[i], i);
            if (i == 1) //Bizim karakter
                ActionHelpers.Instance.SetButtonsPlayer(_playerScripts[i]);

            break;
        }
    }
    public void CheckAllPlayer()
    {
        for (int i = 0; i < _playerScripts.Count; i++)
            if (!_playerScripts[i].IsFull)
                return;

        //StartGame
    }

}
