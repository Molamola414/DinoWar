using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Used for playing audio effects
/// </summary>
public class AudioPlayer : MonoBehaviour
{
    /// <summary>
    /// List of audio sources, at least one is required. Required for playing many affects simultaneously
    /// </summary>
    public List<AudioSource> AudioSources;

    /// <summary>
    /// List of audio clips
    /// </summary>
    public AudioClip[] Effects;

    public static AudioPlayer Instance;

    public void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Play sound effect by name
    /// </summary>
    /// <param name="effect"></param>
    public static void PlayEffect(string effect)
    {
        foreach (var audioSource in Instance.AudioSources)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(Instance.Effects.Single(i => i.name == effect));
                return;
            }
        }
    }
}
