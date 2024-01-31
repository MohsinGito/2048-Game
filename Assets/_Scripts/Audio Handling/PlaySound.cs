using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Audio;

public class PlaySound : MonoBehaviour
{

    [SerializeField] private AudioName sound;

    private void Start()
    {
        if (AudioController.Instance != null)
            AudioController.Instance.PlayAudio(sound);
    }
}
