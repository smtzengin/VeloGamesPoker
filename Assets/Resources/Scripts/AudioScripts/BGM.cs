using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public static BGM Instance;
    private AudioSource _source;
    int _index = 0;
    [SerializeField] private AudioClip[] _clips;
    
    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void ChangeSong()
    {
        _index = (_index + 1) % _clips.Length;
        _source.clip = _clips[_index];
        _source.Play();
    }
}
