using UnityEngine;
using Utilities.Audio;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{

    [SerializeField] private AudioName clickSound = AudioName.BUTTON_CLICK;
    private Button b;

    void Start()
    {
        b = GetComponent<Button>();
        if (b)
        {
            b.onClick.RemoveListener(ClickSound);
            b.onClick.AddListener(ClickSound);
        }
    }

    public void ClickSound()
    {
        AudioController.Instance.PlayAudio(clickSound);
    }

}