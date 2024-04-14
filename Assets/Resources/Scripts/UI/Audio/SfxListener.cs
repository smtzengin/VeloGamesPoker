using UnityEngine;
using UnityEngine.UI;

public class SfxListener : SliderListener
{
    private Slider _slider;
    public override void Initialize()
    {
        _slider = GetComponent<Slider>();
        _slider.value = PlayerPrefs.GetFloat("Sfx");
        AudioManager.ChangeSfx(_slider);
        _slider.onValueChanged.AddListener( delegate { AudioManager.ChangeSfx(_slider); } );
    }
}
