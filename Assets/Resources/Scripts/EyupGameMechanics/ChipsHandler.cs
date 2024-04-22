using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChipsHandler : MonoBehaviour
{
    public static ChipsHandler Instance;

    private const int MAX_CHIPS = 30;
    private Transform[] _chips;

    [SerializeField] private GameObject _chipPrefab;
    [SerializeField] private float _chipsHeight;

    private void Awake() { Instance = this; CreateChips(); }


    public GameObject BidChips(Player p)
    {
        int counter = 0;
        int amount = p.GetLastBid() / 20;
        int loopCount = (int)Mathf.Ceil(amount / 5f);
        int chipCount = (amount % 5) == 0 ? 5 : (amount % 5);
        float chipBlockOffset = 0;
        float chipsHeight = _chipsHeight;

        GameObject chips = new GameObject();
        chips.transform.position = p.GetDealerTransform().position;

        for (int i = 0; i < loopCount; i++)
        {
            for (int j = 0; j < chipCount; amount--, counter++)
            {
                Debug.Log($"Amount: {amount}  loopCount: {loopCount} chipCount: {chipCount}");
                
                Transform chip = _chips[counter++];
                chip.transform.position = p.GetDealerTransform().position + (p.transform.right * chipBlockOffset);
                chip.transform.SetParent(chips.transform);
                chip.gameObject.SetActive(true);
                chip.transform.position += Vector3.up * chipsHeight;
                
                chipsHeight *= 2;
                chipCount--;
            }
            chipsHeight = _chipsHeight;
            chipBlockOffset += 0.5f;
            chipCount = (amount % 5) == 0 ? 5 : (amount % 5);
        }
        return chips;
    }


    private void CreateChips()
    {
        _chips = new Transform[MAX_CHIPS];

        for (int i = 0; i < MAX_CHIPS; i++)
        {
            _chips[i] = Instantiate(_chipPrefab, Vector3.zero, Quaternion.identity).transform;
            _chips[i].gameObject.SetActive(false);
        }
    }
}
