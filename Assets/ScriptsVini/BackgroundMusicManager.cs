using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;
    private AudioSource audioSource;

    // Garante que esta instância seja única (Singleton) e persista entre cenas.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Inicia a reprodução da música caso não esteja tocando.
    void Start()
    {
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = true;
            
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    // Interrompe a reprodução da música.
    public void StopMusic()
    {
        if (audioSource != null) audioSource.Stop();
    }
    
    // Ajusta o volume da música.
    public void SetVolume(float volume)
    {
        if (audioSource != null) audioSource.volume = volume;
    }
}