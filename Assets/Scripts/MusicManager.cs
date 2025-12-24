using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource source;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        source.volume = volume;
        source.Play();
    }
    public void SetVolume(float value)
    {
        source.volume = value;
    }
}
