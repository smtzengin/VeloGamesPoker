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
    public static bool _isAllIn;
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
        _allInOne.onClick.AddListener(delegate { AllIn(_player);Choosed(); });
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

        if (p.GetCurrentBid() < GameLoopManager.Instance.MinBet) //Player'ýn son bahsi bir önceki oyuncudan düþükse
            amount = GameLoopManager.Instance.MinBet - p.GetCurrentBid(); //aradaki farký miktara ekle

        if (p.GetChips() < amount) //Player'ýn yeteri kadar chipi yoksa bu seçeneði engelle!
            return;

        p.AddBid(amount); //Player'ýn son bahsini yükselt

        GameLoopManager.Instance.CurrentBet += amount; //Oyundaki toplam bahsi yükselt.
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
    public void AllIn(Player player)
    {
        AudioManager.PlayAudio(_betSfx);
        int chips = player.GetChips();
        player.AddBid(chips);
        GameLoopManager.Instance.CurrentBet += chips;
        GameLoopManager.Instance.MinBet += chips;
        Debug.Log(GameLoopManager.Instance.MinBet + "" + GameLoopManager.Instance.CurrentBet);
        _isAllIn = true;
        player.DidAllIn = true;
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

        if (playerChips == 0)
        {
            CheckAllow(true); // Check yapma yeteneðini etkinleþtir
            ButtonActive(0, true); // fold butonunu kontrol et
            ButtonActive(1, true); // Call butonunu devre dýþý býrak
            ButtonActive(2, false); // All-in butonunu devre dýþý býrak
            ButtonActive(3, false); // Raise butonunu devre dýþý býrak
            ButtonActive(4, false); // increase butonunu devre dýþý býrak
            ButtonActive(5, false); // decrease butonunu devre dýþý býrak
        }
        else if (playerChips > 0 && playerChips + playerBet < GameLoopManager.Instance.MinBet) 
        {
            CheckAllow(true); // Check yapma yeteneðini etkinleþtir
            ButtonActive(0, true); // fold butonunu acik tut
            ButtonActive(1, false); // Call butonunu devre dýþý býrak
            ButtonActive(2, true); // All-in butonunu kontrol et
            ButtonActive(3, false); // Raise butonunu devre dýþý býrak
            ButtonActive(4, false); // Raise butonunu devre dýþý býrak
            ButtonActive(5, false); // Raise butonunu devre dýþý býrak
        }
        else if (GameLoopManager.Instance.GetLastPlayer().GetCurrentBid() == p.GetCurrentBid())
        {
            // Eðer son oyuncunun bahisi, mevcut oyuncunun bahis miktarýna eþitse,
            // raise yapabilir veya mevcut bahisi eþitleyerek call yapabilir.
            CheckAllow(true); // Check yapma yeteneðini etkinleþtir
            ButtonActive(2, playerChips >= GameLoopManager.Instance.MinBet); // All-in butonunu kontrol et
            ButtonActive(3, true); // Raise butonunu etkinleþtir
        }
        else
        {
            // Diðer durumlarda raise yapabilir
            CheckAllow(false);
            ButtonActive(2, playerChips >= GameLoopManager.Instance.MinBet); // All-in butonunu kontrol et
            ButtonActive(3, true); // Raise butonunu etkinleþtir
        }
        // Diðer butonlarýn durumunu kontrol et
        ButtonActive(5, playerChips >= _raiseAmount + 40); // Raise miktarýný artýrma butonunu ayarla
        ButtonActive(4, _raiseAmount != 40); // Raise miktarýný azaltma butonunu ayarla
    }
    private void CheckAllow(bool isCheck)
    {
        _isCheck = isCheck;
        if (isCheck)
            UIManager.CallCheckText(true); // Call yazýsý yerine Check yazýsý
        else
            UIManager.CallCheckText(false); // Diðer durumlar için Call yazýsý
    }
    private void ButtonActive(int line, bool active)
    {
        UIManager.ButtonActive(line, active);
    }
}
