using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Pathfinding;

public enum GamePhase
{
    init,
    game,
    complete,
    pause
}

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    public GamePhase phase = GamePhase.init;

    public List<Letter> word;
    public AIPath player;
    public AIPath enemy;
    public Init init;
    
    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);
        
        init = GetComponent<Init>();
    }

    public void StartGame()
    {
        if (word == null) word = new List<Letter>();
        foreach (Letter l in init.lets)
        {
            l.takeNotify += AddToWord;
            l.cancelNotify += RemoveAtWord;
        }
        phase = GamePhase.game;
    }
    
    public void CompleteGame()
    {
        phase = GamePhase.complete;
    }
    
    private void RemoveAtWord(Letter l)
    {
        if (word != null && word.Contains(l))
        {
            word.Remove(l);
            StartCoroutine(Move(l, l.posLet));
        }
    } 
    
    private void AddToWord(Letter l)
    {
        if (word != null && !word.Contains(l))
        {
            word.Add(l);
            StartCoroutine(Move(l, new Vector2(0, -4f)));
        } 
    }

    private IEnumerator Move(Letter l, Vector2 target)
    {
        float step = 3f * Time.deltaTime;
        while (Vector2.Distance(l.transform.position, target) > float.Epsilon)
        {
            l.transform.position = Vector2.MoveTowards(l.transform.position, target, step);
            yield return null;
        }
    }
    
    void Update()
    {
        if (phase != GamePhase.game)
        {
            player.canMove = false;
            enemy.canMove = false;
        } 
        else
        {
            player.canMove = true;
            enemy.canMove = true;
        }
    }
}
