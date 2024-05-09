using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    const int START_AUDIO_SOURCES = 2;

    private static AudioManager instance;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private SliderListener[] _sliders;
    private List<AudioSource> _sources;
    private int _audioLine;

    void Awake()
    {
        instance = this;
        Initialize();
    }

    private void Initialize()
    {
        _sources = new List<AudioSource>();

        for (int i = 0; i < START_AUDIO_SOURCES; i++)
        {
            _sources.Add(gameObject.AddComponent<AudioSource>());
            _sources[i].outputAudioMixerGroup = _audioMixer.FindMatchingGroups("Sfx")[0];
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetFloat("Music", 0.5f);
            PlayerPrefs.SetFloat("Sfx", 1f);
        }

        for (int i = 0; i < _sliders.Length; i++)
            if (_sliders[i] != null)
                _sliders[i].Initialize();


    }
    public static void ChangeMusic(Slider slider)
    {
        if (slider.value <= 0)
            slider.value = 0.0001f;

        instance._audioMixer.SetFloat("Music", Mathf.Log10(slider.value) * 20);
        PlayerPrefs.SetFloat("Music", slider.value);
    }
    public static void ChangeSfx(Slider slider)
    {
        if (slider.value <= 0)
            slider.value = 0.0001f;

        instance._audioMixer.SetFloat("Sfx", Mathf.Log10(slider.value) * 20);
        PlayerPrefs.SetFloat("Sfx", slider.value);
    }
    public static void PlayAudio(AudioClip clip) => instance.PlayAudio(clip, 0);
    private void PlayAudio(AudioClip clip, int attempt)
    {
        if (attempt == _sources.Count)
        {
            _sources.Add(gameObject.AddComponent<AudioSource>());
            _sources[attempt].clip = clip;
            _sources[attempt].outputAudioMixerGroup = _audioMixer.FindMatchingGroups("Sfx")[0];
            _sources[attempt].Play();
            _audioLine = attempt;
            return;
        }
        else if (!_sources[_audioLine % _sources.Count].isPlaying)
        {
            _sources[_audioLine % _sources.Count].clip = clip;
            _sources[_audioLine++ % _sources.Count].Play();
            return;
        }
        else
        {
            _audioLine++;
            PlayAudio(clip, attempt + 1);
        }
    }
}
