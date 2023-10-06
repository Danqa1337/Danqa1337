using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private SwitchSpriteOnClick _soundButton;

    private void Start()
    {
        if (GetVolume() == -80)
        {
            _soundButton.Switch();
        }
    }

    private float GetVolume()
    {
        var volume = 0f;
        _audioMixer.GetFloat("Volume", out volume);
        return volume;
    }

    public void SwitchMute()
    {
        var volume = GetVolume();

        if (volume == -80)
        {
            volume = 0;
        }
        else
        {
            volume = -80;
        }
        _audioMixer.SetFloat("Volume", volume);
        Debug.Log(volume);
    }
}