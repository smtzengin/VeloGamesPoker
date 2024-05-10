using UnityEngine;
using UnityEngine.UI;

public class MusicListener : SliderListener
{
    private Slider _slider;
    public override void Initialize()
    {
        _slider = GetComponent<Slider>();
        _slider.value = PlayerPrefs.GetFloat("Music");
        AudioManager.ChangeMusic(_slider);
        _slider.onValueChanged.AddListener( delegate { AudioManager.ChangeMusic(_slider); } );
    }
}
