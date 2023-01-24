using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GamePhase
{
    game,
    complete,
    pause
}

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    public GamePhase phase = GamePhase.init;
    public IAstarAI enemy;
    public Init init;
    
    public delegate void Take(Letter l);
    public event Take takeNotify, cancelNotify;
    
    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);
        
        init = GetComponent<Init>();
    }

    public void StartGame()
    {
        takeNotify += AddToWord;
        cancelNotify += RemoveAtWord;
        phase = GamePhase.game;
    }
    
    public void CompleteGame()
    {
        phase = GamePhase.complete;
    }
    
    private void RemoveAtWord(Letter l)
    {
        if (init.lets.Contains(l)) init.lets.Remove(l);
        StartCoroutine(Move(l, l.posLet));
    } 
    
    private void AddToWord(Letter l)
    {
        if (!init.lets.Contains(l)) init.lets.Add(l);
        StartCoroutine(Move(l, new Vector2(0, -5f)));
    }

    private IEnumerator Move(Letter l, Vector2 target)
    {
        float step = 5f * Time.deltaTime;
        while (Vector2.Distance(l.transform.position, target) > float.Epsilon)
        {
            l.transform.position = Vector2.MoveTowards(l.transform.position, target, step);
            yield return null;
        }
    }
    
    void Update()
    {
        if (phase != GamePhase.game) enemy.gameObject.SetActive(false);
        else enemy.gameObject.SetActive(true);
    }
    
    public static Coroutine Invoke(this MonoBehaviour monoBehaviour, Action action, float time)
    {
        return monoBehaviour.StartCoroutine(InvokeAct(action, time));
    }
    
    private static IEnumerator InvokeAct(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
