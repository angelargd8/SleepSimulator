using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour

{

    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Música por escena")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip bedroomMusic;
    [SerializeField] private AudioClip roadRushMusic;
    [SerializeField] private AudioClip lucidDreamMusic;
    [SerializeField] private AudioClip loadingMusic;

    private AudioSource sfxAudio => GetComponents<AudioSource>()[0];
    private AudioSource ambienceAudio => GetComponents<AudioSource>()[1];
    private AudioSource loopSfxAudio => GetComponents<AudioSource>()[2];

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LoadingScene")
            return;
        PlayMusicForScene(scene.name);
    }

    public void PlayLoadingMusic()
    {
        PlayMusicForScene("LoadingScene");
    }

    public void PlayMusicForScene(string sceneName)
    {
        AudioClip newClip = null;

        switch (sceneName)
        {
            case "MainMenu":
                newClip = mainMenuMusic;
                break;

            case "SceneMenu":
                newClip = mainMenuMusic;
                break;

            case "Bedroom":
                newClip = bedroomMusic;
                break;

            case "RoadRush":
                newClip = roadRushMusic;
                break;

            case "LucidDream":
                newClip = lucidDreamMusic;
                break;

            case "LoadingScene":
                newClip = loadingMusic;
                break;

            default:
                Debug.LogWarning("No hay música configurada para la escena: " + sceneName);
                return;
        }

        if (newClip == null)
        {
            Debug.LogWarning("El AudioClip de la escena " + sceneName + " no está asignado.");
            return;
        }

        if (musicSource.clip == newClip && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        Debug.Log("Reproduciendo música de escena: " + sceneName);
    }

    public float MasterVolume
    {
        get
        {
            float vol;
            audioMixer.GetFloat("MasterVolume", out vol);
            vol = (vol + 80.0f) / 80.0f;
            return vol;
        }
    }

    public float AmbienceVolume
    {
        get
        {
            float vol;
            audioMixer.GetFloat("AmbienceVolume", out vol);
            vol = (vol + 80.0f) / 80.0f;
            return vol;
        }
    }

    public float SFXVolume
    {
        get
        {
            float vol;
            audioMixer.GetFloat("SFXVolume", out vol);
            vol = (vol + 80.0f) / 80.0f;
            return vol;
        }

    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxAudio.PlayOneShot(clip, 5.0f);
    }

    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null) return;

        ambienceAudio.Stop();
        ambienceAudio.clip = clip;
        ambienceAudio.loop = true;
        ambienceAudio.Play();
    }

    public void StopAmbience()
    {
        ambienceAudio.Stop();
    }

    public void PlayLoopSFX(AudioClip clip)
    {
        if (clip == null) return;

        if (loopSfxAudio.clip == clip && loopSfxAudio.isPlaying)
        {
            return;
        }

        loopSfxAudio.Stop();
        loopSfxAudio.clip = clip;
        loopSfxAudio.loop = true;
        loopSfxAudio.Play();
    }

    public void StopLoopSFX(AudioClip clip)
    {
        if (loopSfxAudio.clip == clip)
        {
            loopSfxAudio.Stop();
            loopSfxAudio.clip = null;
        }
    }

    public void StopSFX()
    {
        sfxAudio.Stop();
    }

    public bool IsSFXPlaying(AudioClip clip)
    {
        return loopSfxAudio.clip == clip && loopSfxAudio.isPlaying;
    }

    public void SetMasterVolume(float vol)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80f, 0f, vol));
    }

    public void SetAmbienceVolume(float vol)
    {
        audioMixer.SetFloat("AmbienceVolume", Mathf.Lerp(-80f, 0f, vol));
    }

    public void SetSFXVolume(float vol)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Lerp(-80f, 0f, vol));
    }


}
