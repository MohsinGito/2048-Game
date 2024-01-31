using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using Utilities.Audio;

public class SettingsUI : MonoBehaviour
{

    [SerializeField] private Image soundImage;
    [SerializeField] private Image musicImage;
    [SerializeField] private Sprite enableSprite;
    [SerializeField] private Sprite disableSprite;
    [SerializeField] private CanvasGroup panelFade;

    private Dictionary<AudioType, Image> audioTypeImages;
    private Dictionary<AudioType, bool> audioStates;

    private void Awake()
    {
        audioStates = new Dictionary<AudioType, bool>();
        audioTypeImages = new Dictionary<AudioType, Image>()
        {
            { AudioType.Sound, soundImage },
            { AudioType.Music, musicImage }
        };
    }

    public void Display()
    {
        audioStates[AudioType.Sound] = SessionManager.Instance.SoundActive;
        audioStates[AudioType.Music] = SessionManager.Instance.MusicActive;
        audioTypeImages[AudioType.Sound].sprite = audioStates[AudioType.Sound] ? enableSprite : disableSprite;
        audioTypeImages[AudioType.Music].sprite = audioStates[AudioType.Music] ? enableSprite : disableSprite;

        panelFade.alpha = 0;
        panelFade.gameObject.SetActive(true);
        panelFade.DOFade(1, 0.25f);
    }

    public void Hide()
    {
        panelFade.DOFade(0, 0.25f).OnComplete(() => panelFade.gameObject.SetActive(false));
    }

    public void AudioButtonClick(int audioType)
    {
        AudioType type = (AudioType)audioType;
        ToggleAudioState(type);
    }

    private void ToggleAudioState(AudioType type)
    {
        audioStates[type] = !audioStates[type];
        audioTypeImages[type].sprite = audioStates[type] ? enableSprite : disableSprite;

        switch (type)
        {
            case AudioType.Sound:
                SessionManager.Instance.SetSound(audioStates[type]);
                AudioController.Instance.MuteSFX(!audioStates[type]);
                break;
            case AudioType.Music:
                SessionManager.Instance.SetMusic(audioStates[type]);
                AudioController.Instance.MuteMusic(!audioStates[type]);
                break;
        }
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}

public enum AudioType
{
    Sound = 0,
    Music = 1
}