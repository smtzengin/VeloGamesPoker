using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private int _chairNo;
    private Player _parent;
    [SerializeField] private GameObject[] _visualCards;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void SetInformation(int no, Player p)
    {
        _chairNo = no;
        _animator.SetInteger("Chair", _chairNo);
        _parent = p;
    }
    private void CreateNextPlayer()
    {
        GameManager.Instance.CreateNewPlayer();
    }
    private void OpenSaloonDoor()
    {
        AnimationHandler.Instance.OpenSaloonDoor();
    }
    private void Sit()
    {
        _animator.SetTrigger("Sit");
        _animator.SetFloat("SitPosX", Random.Range(0, 2) == 0 ? -1 : 1);
        _animator.SetFloat("SitPosY", Random.Range(0, 2) == 0 ? -1 : 1);
        _animator.applyRootMotion = true;
        transform.parent = _parent.transform;
        transform.position = _parent.transform.position;
        transform.rotation = _parent.transform.rotation;
        _parent.SetVisualCards(_visualCards);

        // _animator.applyRootMotion = true;

    }
    private void TryToStart()
    {
        GameLoopManager.Instance.TryToStart(createNew: false);
    }
    private void Sat()
    {
        _parent.SetSeatTo(true);
        _parent.PlayerCanvasSetActive(true);
        transform.position = _parent.transform.position;
        transform.rotation = _parent.transform.rotation;
    }
    public void BidTrigger() { _animator.SetTrigger("Bid"); }
    public void FoldTrigger()
    {
        _animator.SetTrigger("Fold");
        _parent.PlayerCanvasSetActive(false);
        //eðer çipler bittiyse destroy çaðýr.
    }
    private void Fold()
    {
        _parent.SetSeatTo(false);
        ActionHelpers.Instance.Fold(_parent);
    }
    private void TryToDestroyCharacter()
    {
        if (_parent.GetChips() == 0)
        {
            _parent.SetCharacter(null);
            if (_parent.IsLocalPlayer)
            {
                UIManager.ToggleEndPanel(false);
                return;
            }
            Destroy(gameObject, 3f);
        }
        else
            Fold();
    }
    public void BackToTrigger()
    {
        _animator.SetTrigger("BackToTable");
    }
    private void BackToTable()
    {
        _parent.SetSeatTo(true);
        _parent.PlayerCanvasSetActive(true);
        TryToStart();
    }

    public void CheckTrigger()
    {
        _animator.SetTrigger("Check");
    }
    private void AnimationComplete()
    {
        GameLoopManager.Instance.OnPlayerAction();
    }
    private void PassBids()
    {
        GameManager.Instance.SendBids(_parent);
    }
}
