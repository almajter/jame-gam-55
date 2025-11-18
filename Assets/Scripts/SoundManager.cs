using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private List<AudioClip> audioClips;

    private AudioClip audioClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayClickSFX()
    {
        if (audioClips.Count == 0) return;
        int index = Random.Range(0, audioClips.Count);
        sfxAudioSource.PlayOneShot(audioClips[index]);
    }
}
