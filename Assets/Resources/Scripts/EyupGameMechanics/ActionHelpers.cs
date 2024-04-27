using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionHelpers : MonoBehaviour
{
    public static ActionHelpers Instance;

    [SerializeField] private Button _fold, _call, _raise, _allInOne, _increaseBid, _decreaseBid;
    private Button[] _buttons;
    private Player _player;
    [SerializeField] private Text _raiseText;
    private int _raiseAmount = 40;
    private bool _isCheck = false;
    private void Awake()
    {
        Instance = this;
        SetButtonList();
    }
    private void SetButtonList()
    {
        _buttons = new Button[] { _fold, _call, _raise, _allInOne, _increaseBid, _decreaseBid };
    }
    public void SetButtonsPlayer(Player p)
    {
        _player = p;
        _fold.onClick.AddListener(delegate { Fold(_player); });
        _call.onClick.AddListener(delegate { Call(_player); });
        _raise.onClick.AddListener(delegate { Raise(_player, _raiseAmount); });
        _increaseBid.onClick.AddListener(delegate { IncreaseBid(); });
        _decreaseBid.onClick.AddListener(delegate { DecreaseBid(); });
    }
    public void Fold(Player p)
    {
        GameLoopManager.Instance.RemovePlayer(p);
        GameLoopManager.Instance.OnPlayerAction();
    }
    public void Call(Player p)
    {
        if (_isCheck)
        {
            Check(p);
            return;
        }
        int amount = 0;

        if (p.GetCurrentBid() < GameLoopManager.Instance.MinBid) //Player'ýn son bahsi bir önceki oyuncudan düþükse
            amount = GameLoopManager.Instance.MinBid - p.GetCurrentBid(); //aradaki farký miktara ekle

        if (p.GetChips() < amount) //Player'ýn yeteri kadar chipi yoksa bu seçeneði engelle!
            return;

        p.AddBid(amount); //Player'ýn son bahsini yükselt

        GameLoopManager.Instance.CurrentBid += amount; //Oyundaki toplam bahsi yükselt.
    }
    public void Check(Player p)
    {
        GameLoopManager.Instance.OnPlayerAction();
    }
    public void Raise(Player p, int amount)
    {
        int newBid = 0;

        if (p.GetChips() == 0 || amount <= 0)
        {
            Debug.Log("Not enough chips to raise.");
            return;
        }

        if (p.GetChips() < amount)
            amount = p.GetChips();

        if (p.GetCurrentBid() <= GameLoopManager.Instance.MinBid)
            newBid = (GameLoopManager.Instance.MinBid - p.GetCurrentBid()) + amount;

        Debug.Log("MinBid: " + GameLoopManager.Instance.MinBid + " PlayerBid: " + p.GetCurrentBid());
        Debug.Log("NewBid: " + newBid);
        Debug.Log("CurrentBid: " + GameLoopManager.Instance.CurrentBid);
        p.AddBid(newBid);

        if (p.IsLocalPlayer)
        {
            _raiseAmount = 40;
            _raiseText.text = _raiseAmount.ToString();
            CheckChips(p);
        }
        GameLoopManager.Instance.CurrentBid += newBid;
        GameLoopManager.Instance.MinBid += amount;
    }
    public void AllIn(Player player)
    {
        int chips = player.GetChips();
        player.AddBid(chips);
        GameLoopManager.Instance.CurrentBid += chips;
    }
    public void IncreaseBid()
    {
        //Check if amount is bigger than player's chips, else
        if (_player.GetChips() < _raiseAmount + 40)
            return;

        _raiseAmount += 40;
        _raiseText.text = _raiseAmount.ToString();

        CheckChips(GameLoopManager.Instance.GetCurrentPlayer());
    }
    public void DecreaseBid()
    {
        if (_raiseAmount != 40)
        {
            _raiseAmount -= 40;
            _raiseText.text = _raiseAmount.ToString();
        }

        CheckChips(GameLoopManager.Instance.GetCurrentPlayer());
    }

    public void CheckChips(Player p)
    {
        if (!p.IsLocalPlayer)
            return;

        int playerChips = p.GetChips();
        int playerBet = p.GetCurrentBid();
        if (playerChips + playerBet < GameLoopManager.Instance.MinBid)
        {
            CheckAllow(true);
            ButtonActive(3, false);
        }
        else if (GameLoopManager.Instance.GetLastPlayer().GetCurrentBid() == p.GetCurrentBid())
            CheckAllow(true);
        else
        {
            CheckAllow(false);
            ButtonActive(3, true);
        }
        if (playerChips < _raiseAmount + 40)
            ButtonActive(5, false);
        else
            ButtonActive(5, true);

        if (_raiseAmount - 40 == 0)
            ButtonActive(4, false);
        else
            ButtonActive(4, true);

    }
    private void CheckAllow(bool isCheck)
    {
        _isCheck = isCheck;
        UIManager.CallCheckText(isCheck);
    }
    private void ButtonActive(int line, bool active)
    {
        UIManager.ButtonActive(line, active);
    }
}
