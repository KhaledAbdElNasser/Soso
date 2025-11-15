using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Card SFX")]
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    public bool randomPitch = true;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    private float GetRandomPitch()
    {
        return randomPitch ? Random.Range(pitchMin, pitchMax) : 1f;
    }

    public void PlayFlip()
    {
        if (flipSound != null)
        {
            audioSource.pitch = GetRandomPitch();
            audioSource.PlayOneShot(flipSound, volume);
        }
    }

    public void PlayMatch()
    {
        if (matchSound != null)
        {
            audioSource.pitch = GetRandomPitch();
            audioSource.PlayOneShot(matchSound, volume);
        }
    }

    public void PlayMismatch()
    {
        if (mismatchSound != null)
        {
            audioSource.pitch = GetRandomPitch();
            audioSource.PlayOneShot(mismatchSound, volume);
        }
    }

}
