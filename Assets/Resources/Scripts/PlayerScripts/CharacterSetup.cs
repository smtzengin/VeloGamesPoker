using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSetup : MonoBehaviour
{
    public static CharacterSetup Instance;
    [SerializeField] private Material[] _characterMaterials;
    [SerializeField] private GameObject[] _characters;
    [SerializeField] private Vector3 _startPos;

    private List<GameObject> _useableCharacters;
    private void Awake()
    {
        Instance = this;
        _useableCharacters = new List<GameObject>(_characters);
    }
    public GameObject CreateCharacter(Player parent, int chairNo, Player player)
    {
        GameObject newPlayer = Instantiate(_useableCharacters[Random.Range(0, _useableCharacters.Count)], _startPos, Quaternion.identity);
        SetupPlayer(newPlayer, parent, chairNo, player);
        return newPlayer;
    }
    public void SetupPlayer(GameObject newPlayer, Player parent, int chairNo, Player player)
    {
        if (player == parent) //Get Player Chip from database
            player.GetChipDataForPlayer();

        newPlayer.GetComponent<PlayerAnimation>().SetInformation(chairNo, parent);
        newPlayer.transform.GetChild(0).GetComponent<Renderer>().material = _characterMaterials[Random.Range(0, _characterMaterials.Length)];
    }
}
