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
    
    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);
    }

    public void StartGame()
    {
        phase = GamePhase.game;
    }
    
    public void CompleteGame()
    {
        phase = GamePhase.complete;
    }
    
    public static Coroutine Invoke(this MonoBehaviour monoBehaviour, Action action, float time)
    {
        return monoBehaviour.StartCoroutine(InvokeAct(action, time));
    }

    void Update()
    {
        if (phase != GamePhase.game) enemy.gameObject.SetActive(false);
        else enemy.gameObject.SetActive(true);
    }

    private static IEnumerator InvokeAct(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
