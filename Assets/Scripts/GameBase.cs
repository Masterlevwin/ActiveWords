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

    public AIPath player;
    public AIPath enemy;
    public Init init;
    public GameObject cellPrefab;
    
    public Dictionary<Vector2, Letter> letDict;
    
    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);
        
        init = GetComponent<Init>();
    }

    public void StartGame(char[] chars)
    {
        if (letDict == null) letDict = new Dictionary<Vector2, Letter>();
        letDict.Count = chars.Length;
        
        if (GameObject.Find("Cells") == null) GameObject cellGO = new GameObject("Cells");
        Letter l = null;
        Vector2 cell = Vector2.zero;
        for (int i = 0; i < letDict.Count; i++) 
        {
            float v = letDict.Count/2;
            cell = new Vector2(.5f - v + i, -4f);
            Instantiate(cellPrefab, cell, cellGO.transform);
            letDict.Add(cell, l);
        }

        foreach (Letter l in init.lets)
        {
            l.takeNotify += AddToWord;
            l.cancelNotify += RemoveAtWord;
        }
        phase = GamePhase.game;
    }
    
    private void CompleteGame()
    {
        phase = GamePhase.complete;
    }
    
    private void RemoveAtWord(Letter l)
    {
        if (letDict.ContainsValue(l))
        {
            letDict.Remove(l.transform.position);
            StartCoroutine(Move(l, l.posLet);
        }
    } 
    
    private void AddToWord(Letter l)
    {
        for (int i = 0; i < letDict.Count; i++)
        {
            if (!letDict.ContainsValue(l) && !letDict.ContainsKey(letDict.ElementAt(i).Key))
            {
                letDict.Add(letDict.ElementAt(i).Key, l);
                StartCoroutine(Move(l, letDict.ElementAt(i).Key));
                break;
            }
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
