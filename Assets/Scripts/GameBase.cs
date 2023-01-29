using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Pathfinding;
using System.Linq;

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
    private Transform cellAnchor;
    private List<Vector2> letPositions;
    private Dictionary<Vector2, Letter> letDict; 

    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);
        
        init = GetComponent<Init>();

        if (GameObject.Find("Cells") == null)
        {
            GameObject cellGO = new GameObject("Cells");
            cellAnchor = cellGO.transform;
        }
    }

    public void StartGame()
    {
        foreach (Transform child in cellAnchor) Destroy(child.gameObject);
        if (letPositions == null) letPositions = new List<Vector2>();
        else letPositions.Clear();
        if (letDict == null) letDict = new Dictionary<Vector2, Letter>();
        else letDict.Clear();
        Vector2 cell = Vector2.zero;
        for (int i = 0; i < init.lets.Count; i++) 
        {
            float v = init.lets.Count/2;
            cell = new Vector2(.5f - v + i, -4.5f);
            Instantiate(cellPrefab, cell, Quaternion.identity, cellAnchor);
            letPositions.Add(cell);
        }
        phase = GamePhase.game;
    }
    
    private void CompleteGame()
    {
        phase = GamePhase.complete;
        // Здесь показать картинки и анимации
    }
    
    public void RemoveAtWord(Letter l)
    {
        if (letDict.ContainsValue(l))
        {
            player.SetPath(null);
            //enemy.SetPath(null);
            letDict.Remove(l.transform.position);
            StartCoroutine(Move(l, l.posLet, true));
        }
    } 
    
    public void AddToWord(Letter l)
    {
        for (int i = 0; i < letPositions.Count; i++)
        { 
            if (!letDict.ContainsValue(l) && !letDict.ContainsKey(letPositions[i]))
            {
                letDict.Add(letPositions[i], l);
                StartCoroutine(Move(l, letPositions[i], false));
                break;
            }
        }
    }

    private IEnumerator Move(Letter l, Vector2 target, bool b)
    {
        float step = 4f * Time.deltaTime;
        while (Vector2.Distance(l.transform.position, target) > float.Epsilon)
        {
            l.transform.position = Vector2.MoveTowards(l.transform.position, target, step);
            yield return null;
        }
        l.GetComponent<BoxCollider2D>().isTrigger = b;
    }
    
    void Update()
    {
        if (letDict != null && letDict.Count == init.lets.Count) CompleteGame();
        
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
