using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionHelpers : MonoBehaviour
{
    public static ActionHelpers Instance;

    [SerializeField] private Button _fold, _call, _raise, _allInOne, _increaseBid, _decreaseBid;
    [SerializeField] private AudioClip _betSfx, _foldSfx;
    private Player _player;
    private int _raiseAmount = 40;
    private bool _isCheck = false;
    private void Awake()
    {
        Instance = this;
    }
    public void SetButtonsPlayer(Player p)
    {
        _player = p;
        _fold.onClick.AddListener(delegate { Fold(_player); Choosed(); });
        _call.onClick.AddListener(delegate { Call(_player); Choosed(); });
        _raise.onClick.AddListener(delegate { Raise(_player, _raiseAmount); Choosed(); });
        _increaseBid.onClick.AddListener(delegate { IncreaseBid();});
        _decreaseBid.onClick.AddListener(delegate { DecreaseBid();});
        _allInOne.onClick.AddListener(delegate { AllIn(_player); });
    }
    private void Choosed()
    {
        UIManager.AllButtonsActive(false);
    }
    public void Fold(Player p)
    {
        AudioManager.PlayAudio(_foldSfx);
        Debug.Log($"{p.name} FOLD.");
        if (p.IsLocalPlayer)
            UIManager.ToggleEndPanel(won: false);
        GameLoopManager.Instance.RemovePlayer(p);
    }
    public void Call(Player p)
    {
        AudioManager.PlayAudio(_betSfx);
        if (_isCheck)
        {
            Check(p);
            return;
        }
        Debug.Log($"{p.name} CALL.");
        int amount = 0;

        if (p.GetCurrentBid() < GameLoopManager.Instance.MinBet) //Player'�n son bahsi bir �nceki oyuncudan d���kse
            amount = GameLoopManager.Instance.MinBet - p.GetCurrentBid(); //aradaki fark� miktara ekle

        if (p.GetChips() < amount) //Player'�n yeteri kadar chipi yoksa bu se�ene�i engelle!
            return;

        p.AddBid(amount); //Player'�n son bahsini y�kselt

        GameLoopManager.Instance.CurrentBet += amount; //Oyundaki toplam bahsi y�kselt.
    }
    public void Check(Player p)
    {
        Debug.Log($"{p.name} CHECK.");
        p.Check();
    }
    public void Raise(Player p, int amount)
    {
        AudioManager.PlayAudio(_betSfx);

        Debug.Log($"{p.name} RAISE {amount}.");
        int newBid = 0;

        if (p.GetChips() == 0 || amount <= 0)
        {
            Debug.Log("Not enough chips to raise.");
            return;
        }

        if (p.GetChips() < amount)
            amount = p.GetChips();

        if (p.GetCurrentBid() <= GameLoopManager.Instance.MinBet)
            newBid = (GameLoopManager.Instance.MinBet - p.GetCurrentBid()) + amount;

        Debug.Log(GameLoopManager.Instance.MinBet + " VE " +p.GetCurrentBid());
        p.AddBid(newBid);

        if (p.IsLocalPlayer)
        {
            _raiseAmount = 40;
            UIManager.UpdateRaiseChipText(_raiseAmount);
        }
        GameLoopManager.Instance.CurrentBet += newBid;
        GameLoopManager.Instance.MinBet += amount;
    }
    private void AICheck(Player p)
    {
        if (p.GetChips() + p.GetCurrentBid() < GameLoopManager.Instance.MinBet)
            _isCheck = true;
        else if (GameLoopManager.Instance.GetLastPlayer().GetCurrentBid() == p.GetCurrentBid())
            _isCheck = true;
        else
            _isCheck = false;
    }
    public void AllIn(Player player)
    {
        AudioManager.PlayAudio(_betSfx);
        int chips = player.GetChips();
        player.AddBid(chips);
        GameLoopManager.Instance.CurrentBet += chips;
    }
    public void IncreaseBid()
    {
        //Check if amount is bigger than player's chips, else
        if (_player.GetChips() < _raiseAmount + 40)
            return;

        _raiseAmount += 40;
        UIManager.UpdateRaiseChipText(_raiseAmount);
        CheckChips(GameLoopManager.Instance.GetCurrentPlayer());
    }
    public void DecreaseBid()
    {
        if (_raiseAmount != 40)
        {
            _raiseAmount -= 40;
            UIManager.UpdateRaiseChipText(_raiseAmount);
        }

        CheckChips(GameLoopManager.Instance.GetCurrentPlayer());
    }

    public void CheckChips(Player p)
    {
        if (!p.IsLocalPlayer)
        {
            UIManager.AllButtonsActive(false);
            AICheck(p);
            return;
        }

        int playerChips = p.GetChips();
        int playerBet = p.GetCurrentBid();
        if (playerChips + playerBet < GameLoopManager.Instance.MinBet)
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