using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public string mainFolder = "GameSounds";
    public string soundFolder = "Sounds";
    public string musicFolder = "Music";

    public float fadeSpeed = 3;
    public Slider userVolume;

    private AudioSource camMusic;

    public AudioMixerGroup musicGroup;
    public AudioMixerGroup soundGroup;

    private static SoundManager _instance;
    private static AudioSource last, current;
    private static float musicVolume, soundVolume;
    private static bool muteMusic, muteSound;

    void Awake()
    {
        musicVolume = 1;
        soundVolume = 1;
        _instance = this;

        camMusic = GetComponent<AudioSource>();
    }

    private void Update()
    {
        soundVolume = userVolume.value;
        camMusic.volume = userVolume.value - .3f;
    }

    public static void SoundVolume(float volume)
    {
        soundVolume = volume;
    }

    public static void MusicVolume(float volume)
    {
        musicVolume = volume;
        if (current) current.volume = volume;
    }

    public static void MuteSound(bool value)
    {
        muteSound = value;
    }

    public static void MuteMusic(bool value)
    {
        muteMusic = value;
        if (current) current.mute = value;
    }

    public static Coroutine _coroutine;
    void PlaySoundInternal(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            Debug.Log(_instance + " :: Sound null.");
            return;
        }
        _coroutine = StartCoroutine(GetSound(soundName));
    }

    public static void StopSound()
    {
        if( _coroutine != null ) _instance.StopCoroutine(_coroutine);
    }

    public static void PlaySound(string name)
    {
        _instance.PlaySoundInternal(name);
    }

    void PlayMusicInternal(string musicName, bool loop)
    {
        if (string.IsNullOrEmpty(musicName))
        {
            Debug.Log(_instance + " :: Music null.");
            return;
        }
        StartCoroutine(GetMusic(musicName, loop));
    }

    public static void PlayMusic(string name, bool loop)
    {
        _instance.PlayMusicInternal(name, loop);
    }

    void LateUpdate()
    {
        Fader();
    }

    void Fader()
    {
        if (last == null) return;

        last.volume = Mathf.Lerp(last.volume, 0, fadeSpeed * Time.deltaTime);
        current.volume = Mathf.Lerp(current.volume, musicVolume, fadeSpeed * Time.deltaTime);

        if (last.volume < 0.05f)
        {
            last.volume = 0;
            Destroy(last.gameObject);
        }
    }

    IEnumerator GetMusic(string musicName, bool loop)
    {
        ResourceRequest request = LoadAsync(musicFolder + "/" + musicName);

        while (!request.isDone)
        {
            yield return null;
        }

        AudioClip clip = (AudioClip)request.asset;

        if (clip == null)
        {
            Debug.Log(_instance + " :: Music get: " + musicName);
            yield return false;
        }

        last = current;

        GameObject obj = new GameObject("Music: " + musicName);
        AudioSource au = obj.AddComponent<AudioSource>();
        obj.transform.parent = transform;
        au.outputAudioMixerGroup = musicGroup;
        au.playOnAwake = false;
        au.loop = loop;
        au.mute = muteMusic;
        au.volume = (last == null) ? musicVolume : 0;
        au.clip = clip;
        au.Play();
        current = au;
    }

    IEnumerator GetSound(string soundName)
    {
        ResourceRequest request = LoadAsync(soundFolder + "/" + soundName);

        while (!request.isDone)
        {
            yield return null;
        }

        AudioClip clip = (AudioClip)request.asset;

        if (clip == null)
        {
            Debug.Log(_instance + " :: Sound get: " + soundName);
            yield return false;
        }

        GameObject obj = new GameObject("Sound: " + soundName);
        AudioSource au = obj.AddComponent<AudioSource>();
        obj.transform.parent = transform;
        au.outputAudioMixerGroup = soundGroup;
        au.playOnAwake = false;
        au.loop = false;
        au.mute = muteSound;
        au.volume = soundVolume;
        au.clip = clip;
        au.Play();
        Destroy(obj, clip.length);
        _coroutine = null;
    }

    ResourceRequest LoadAsync(string name)
    {
        string path = mainFolder + "/" + name;
        return Resources.LoadAsync<AudioClip>(path);
    }
}
