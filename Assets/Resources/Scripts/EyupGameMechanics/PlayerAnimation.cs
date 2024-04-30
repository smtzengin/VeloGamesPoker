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
    private void CreateNextPlayer()
    {
        GameManager.Instance.CreateNewPlayer();
    }
    private void TryToStart()
    {
        GameLoopManager.Instance.TryToStart();
    }
    private void Sat() 
    { 
        _parent.SetSeatTo(true); 
        _parent.PlayerCanvasSetActive(true);
    }
    private void GetUp() { _parent.SetSeatTo(false); }
    public void BidTrigger() {_animator.SetTrigger("Bid"); }
    public void FoldTrigger() 
    {
        _animator.SetTrigger("Fold");
        _parent.PlayerCanvasSetActive(false);
        //e�er �ipler bittiyse destroy �a��r.
    }
    private void DestroyCharacter()
    {
        //Karakterin �ipi biterse �al��
        //DESTROY AND NEW CHARACTER
        Destroy(gameObject);
        CreateNextPlayer();
    }
    public void CheckTrigger() {_animator.SetTrigger("Check"); }
    private void AnimationComplete()
    {
        GameLoopManager.Instance.OnPlayerAction();
    }
    private void PassBids()
    {
        GameManager.Instance.SendBids(_parent);
    }
}
