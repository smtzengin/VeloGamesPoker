using Resources.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGM : MonoBehaviour
{
    public static BGM Instance;
    private AudioSource _source;
    private int _index = 0;
    [SerializeField] MusicList _musicList;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _source = GetComponent<AudioSource>();
        }
    }
    public string ChangeSong(bool right)
    {
        Debug.Log(_musicList.Music.Length);

        if (right)
            _index = (_index + 1) % _musicList.Music.Length;
        else
        {
            if (_index - 1 < 0)
                _index = _musicList.Music.Length - 1;
            else
                _index -= 1;
        }

        _source.clip = _musicList.Music[_index].MusicClip;
        _source.Play();
        return _musicList.Music[_index].MusicName;
    }
    public string GetMusicName()
    {
        return _musicList.Music[_index].MusicName;
    }
    private void Update()
    {
        if (!_source.isPlaying)
        {
            ChangeSong(true);
            TryToChangeText();
        }
    }

    public void TryToChangeText()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            ViewManager.GetView<SettingsView>().DisplayMusicName();
    }
}
[Serializable]
public struct MusicList
{
    public Music[] Music; 
}
[Serializable]
public struct Music
{
    public string MusicName;
    public AudioClip MusicClip;
}