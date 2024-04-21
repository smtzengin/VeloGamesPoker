using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionHelpers : MonoBehaviour
{
    public static ActionHelpers Instance;

    [SerializeField] private Button _fold, _call, _raise, _allInOne, _increaseBid, _decreaseBid;
    private Player _player;
    [SerializeField] private Text _raiseText;
    private int _raiseAmount;
    private void Awake()
    {
        Instance = this;
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
    public void Fold(Player player)
    {
        GameLoopManager.Instance.RemovePlayer(player);
        GameLoopManager.Instance.OnPlayerAction();
    }
    public void Call(Player player)
    {
        int amount = 0;
        
        if (player.GetCurrentBid() < GameLoopManager.Instance.MinBid) //Player'ýn son bahsi bir önceki oyuncudan düþükse
            amount = GameLoopManager.Instance.MinBid - player.GetCurrentBid(); //aradaki farký miktara ekle

        if (player.GetChips() < amount) //Player'ýn yeteri kadar chipi yoksa bu seçeneði engelle!
            return;

        if(amount == 0) //eðer oyuncunun o anki bahsi minbid ile eþit ise -> Skip
        {
        }

        player.AddBid(amount); //Player'ýn son bahsini yükselt

        GameLoopManager.Instance.CurrentBid += amount; //Oyundaki toplam bahsi yükselt.
        GameLoopManager.Instance.OnPlayerAction(); //Sýrayý devam ettir.
    }
    public void Raise(Player player, int amount)
    {
        int newBid = 0;
        if (player.GetCurrentBid() <= GameLoopManager.Instance.MinBid) //Player'ýn son bahsi bir önceki oyuncudan düþükse
            newBid = (GameLoopManager.Instance.MinBid - player.GetCurrentBid()) + amount;

        Debug.Log("MinBid: " + GameLoopManager.Instance.MinBid + " PlayerBid: " + player.GetCurrentBid());
        Debug.Log("NewBid: " + newBid);
        Debug.Log("CurrentBid: " + GameLoopManager.Instance.CurrentBid);
        player.AddBid(newBid); //Player'ýn son bahsini yükselt



        GameLoopManager.Instance.CurrentBid += newBid;
        GameLoopManager.Instance.MinBid += amount;
        GameLoopManager.Instance.OnPlayerAction();
    }
    public void AllIn(Player player)
    {
        //eðer oyuncunun yeteri kadar chip'i varsa:
        //player.getchip
        GameLoopManager.Instance.OnPlayerAction();
    }
    public void IncreaseBid()
    {
        //Check if amount is bigger than player's chips, else
        if (_player.GetChips() < _raiseAmount + 40)
            return;

        _raiseAmount += 40;
        _raiseText.text = _raiseAmount.ToString();
    }
    public void DecreaseBid()
    {
        if(_raiseAmount != 40)
        {
            _raiseAmount -= 40;
            _raiseText.text = _raiseAmount.ToString();
        }
    }

    public void FirstBids(int amount)
    {
        Player p = GameLoopManager.Instance.GetCurrentPlayer();
            Raise(p, amount);
    }



}
