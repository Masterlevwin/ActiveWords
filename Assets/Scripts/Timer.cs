using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private TMP_Text timerText;

    private float _timeLeft = 0f;

    private IEnumerator StartTimer()
    {
        while (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void BeginTimer( Vector2 pos, float rebirth )
    {
        if( !gameObject.activeSelf ) gameObject.SetActive(true);
        transform.position = pos;
        _timeLeft = rebirth;
        StartCoroutine( StartTimer() );
    }

    public void StopTimer()
    {
        _timeLeft = 0f;
        StopCoroutine( StartTimer() );
        timerText.text = $" ";
    }

    private void UpdateTimeText()
    {
        float seconds = Mathf.FloorToInt( _timeLeft % 60 );
        timerText.text = $"{ seconds }";
    }
}
