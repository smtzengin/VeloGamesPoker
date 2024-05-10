using Resources.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public static BGM Instance => Singleton<BGM>.Instance;
    private AudioSource _source;
    private int _index = 0;
    [SerializeField] private AudioClip[] _clips;

    public void ChangeSong()
    {
        _index = (_index + 1) % _clips.Length;
        _source.clip = _clips[_index];
        _source.Play();
    }
}
