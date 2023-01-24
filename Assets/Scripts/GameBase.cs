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
    
    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);
        
        init = GetComponent<Init>();
        foreach (GameObject lGO in init.lets) lGO.takeNotify += MoveToWord;
    }

    public void StartGame(ref List<GameObject> lets)
    {
        foreach (GameObject g in lets) Debug.Log(g.name);
        phase = GamePhase.game;
    }
    
    public void CompleteGame()
    {
        phase = GamePhase.complete;
    }
    
    public void CancelLetter()
    {
        
    }
    
    private void MoveToWord(GameObject l)
    {
        StartCoroutine(Move(l));
    }

    private IEnumerator Move(GameObject l)
    {
        float step = 5f * Time.deltaTime;
        Vector2 target = new Vector2(0, -5f);
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
