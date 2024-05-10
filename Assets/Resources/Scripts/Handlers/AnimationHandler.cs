using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public static AnimationHandler Instance;
    [SerializeField] private Animator _saloonAnimator;
    private void Awake() =>
        Instance = this;

    public void OpenSaloonDoor()
    {
        _saloonAnimator.SetTrigger("Open");
    }
}
