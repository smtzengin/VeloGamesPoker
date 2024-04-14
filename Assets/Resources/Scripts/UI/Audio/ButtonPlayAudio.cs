using UnityEngine;
using UnityEngine.UI;

public class ButtonPlayAudio : MonoBehaviour
{
    [SerializeField] AudioClip _source;

    private void PlayAudio()
    {
        AudioManager.PlayAudio(_source);
    }
    private void Awake() =>
        GetComponent<Button>().onClick.AddListener(PlayAudio);
}
