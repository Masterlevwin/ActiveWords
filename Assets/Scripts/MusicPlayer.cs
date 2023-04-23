using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;
    private static int _lastAudioIndex = 0;

    private Coroutine _audioPlayCoroutine;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (FindObjectsOfType<MusicPlayer>().Length > 1)
        {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();

        _lastAudioIndex = GetClipIndex();
        _audioPlayCoroutine = StartCoroutine(GetAudioPlay());

        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator GetAudioPlay()
    {
        while (true)
        {
            var clip = _audioClips[_lastAudioIndex];

            _audioSource.clip = clip;
            _audioSource.Play();

            yield return new WaitForSeconds(clip.length + Time.deltaTime);

            _audioSource.Stop();

            _lastAudioIndex++;
            _lastAudioIndex = GetClipIndex();
        }
    }

    private int GetClipIndex()
    {
        return _lastAudioIndex % _audioClips.Length;
    }
}
