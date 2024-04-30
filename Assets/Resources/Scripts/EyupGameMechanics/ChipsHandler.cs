using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChipsHandler : MonoBehaviour
{
    public static ChipsHandler Instance;

    private const int MIN_20_CHIPS = 30;
    private const int MIN_40_CHIPS = 30;
    private const int MIN_100_CHIPS = 30;
    private List<Transform> _chips20, _chips40, _chips100;

    [SerializeField] private GameObject _chip20Prefab, _chip40Prefab, _chip100Prefab;
    private Dictionary<Player, GameObject> _playerBetChips = new Dictionary<Player, GameObject>();

    [SerializeField] private Transform _middle;
    private Transform _chipPool;
    private bool _moveToMiddle;

    private void Awake() { Instance = this; CreateChips(); }


    //Player bid animasyonu bittikten sonra çaðýrýlýr
    public GameObject BidChips(Player p)
    {
        int lastBid = p.GetLastBid();

        //Kaç chip gerektiðini hesapla
        int amount100 = CalculateAmount(ref lastBid, 100);
        int amount40 = CalculateAmount(ref lastBid, 40);
        int amount20 = CalculateAmount(ref lastBid, 20);

        //Daha önceden betchips oluþturulmamýþ ise yeni oluþtur 
        if (!_playerBetChips.ContainsKey(p))
            CreateBetChip(p);
        _playerBetChips[p].transform.position = p.GetDealerTransform().position;

        //Yeni transform oluþtur (Pozisyonu hareket ettirmek için) ve chipi yerine koy
        Transform chip;
        for (int i = 0; i < amount100; i++)
        {
            chip = GetChip(_chips100);
            SetChipTransform(chip, p, 2);

        }
        for (int i = 0; i < amount40; i++)
        {
            chip = GetChip(_chips40);
            SetChipTransform(chip, p, 1);
        }
        for (int i = 0; i < amount20; i++)
        {
            chip = GetChip(_chips20);
            SetChipTransform(chip, p, 0);
        }

        return _playerBetChips[p];
    }

    public void NextGameLoop()
    {
        _moveToMiddle = true;
    }
    private void MoveToMiddle()
    {
        bool reached = true;
        //reached true olana kadar tüm playerbetchipleri ortaya doðru hareket ettir.
        int playerCount = GameLoopManager.Instance.GetCurrentPlayers().Count;
        for (int i = 0; i < playerCount; i++)
        {
            Player p = GameLoopManager.Instance.GetCurrentPlayers()[i];
            Vector3 direction = (_middle.transform.position - _playerBetChips[p].transform.position).normalized;
            _playerBetChips[p].transform.position += direction * 3 * Time.deltaTime;

            if (Vector3.Distance(_playerBetChips[p].transform.position, _middle.position) > 0.2f)
                reached = false;
        }
        if (reached)
        {
            for (int i = 0; i < _playerBetChips.Count; i++)
            {
                Player p = GameLoopManager.Instance.GetPlayersInLine()[i];
                for (int j = 0; j < 3; j++)
                {
                    Transform betPoint = _playerBetChips[p].transform.GetChild(j);
                    if (betPoint.childCount > 0)
                    {
                        Transform[] chips = betPoint.GetComponentsInChildren<Transform>();
                        foreach (Transform chip in chips)
                        {
                            if (chip != betPoint)
                            {
                                chip.SetParent(null);
                                chip.gameObject.SetActive(false);
                            }
                        }
                    }

                }
                _playerBetChips[p].transform.position = p.GetDealerTransform().position;
            }

            _moveToMiddle = false;
            GameLoopManager.Instance.Ackard();
        }

    }
    private void Update()
    {
        if (_moveToMiddle)
            MoveToMiddle();
    }
    private void CreateChips()
    {
        //Chip Object Pooling
        _chipPool = new GameObject().transform;

        _chips20 = new List<Transform>();
        _chips40 = new List<Transform>();
        _chips100 = new List<Transform>();

        for (int i = 0; i < MIN_20_CHIPS; i++)
        {
            _chips20.Add(Instantiate(_chip20Prefab, _chipPool).transform);
            _chips20[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < MIN_40_CHIPS; i++)
        {
            _chips40.Add(Instantiate(_chip40Prefab, _chipPool).transform);
            _chips40[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < MIN_100_CHIPS; i++)
        {
            _chips100.Add(Instantiate(_chip100Prefab, _chipPool).transform);
            _chips100[i].gameObject.SetActive(false);
        }
    }
    private Transform AddNewChips(List<Transform> chips)
    {
        int chipCount = chips.Count;

        for (int i = chipCount; i < chipCount + 5; i++)
        {
            chips.Add(Instantiate(chips[0], _chipPool).transform);
            chips[i].gameObject.SetActive(false);
        }
        return chips[chipCount];
    }
    private int CalculateAmount(ref int lastBid, int divider)
    {
        int amount = lastBid / divider;
        lastBid -= amount * divider;
        return amount;
    }

    //Chip object poolingten aktif olmayaný çek.
    private Transform GetChip(List<Transform> chips)
    {
        Transform chip = chips.Find((x) => x.gameObject.activeSelf);
        
        return chip != null ? chip : AddNewChips(chips);
    }
    private void SetChipTransform(Transform chip, Player p, int position)
    {
        chip.transform.position = _playerBetChips[p].transform.GetChild(position).position + (Vector3.up * CalculateChipsHeight(p, position));
        chip.transform.SetParent(_playerBetChips[p].transform.GetChild(position));
        chip.gameObject.SetActive(true);
    }
    private float CalculateChipsHeight(Player p, int pos)
    {
        float newHeight = 0;
        for (int i = 0; i < _playerBetChips[p].transform.GetChild(pos).childCount; i++)
            newHeight += 0.02f;
        return newHeight;
    }
    private void CreateBetChip(Player p)
    {
        _playerBetChips.Add(p, new GameObject
        {
            name = p.name + "CHIPHOLDER"
        });

        GameObject bet20 = new GameObject();
        bet20.name = "20s";
        bet20.transform.SetParent(_playerBetChips[p].transform);
        bet20.transform.localPosition = Vector3.zero + (p.transform.right * -0.1f);

        GameObject bet40 = new GameObject();
        bet40.name = "40s";
        bet40.transform.SetParent(_playerBetChips[p].transform);
        bet40.transform.localPosition = Vector3.zero;

        GameObject bet100 = new GameObject();
        bet100.name = "100s";
        bet100.transform.SetParent(_playerBetChips[p].transform);
        bet100.transform.localPosition = Vector3.zero + (p.transform.right * 0.1f);
    }
}
