using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Pathfinding;
using System.Linq;
using TMPro;

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
    public static int level = 0;
    public TMP_Text levelText;
    
    public AIPath player;
    public AIPath enemy;
    //public GameObject block;
    public Init init;
    public GameObject cellPrefab;
    public Sprite[] cells;
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
        //Instantiate(block, Vector2.zero, Quaternion.identity, transform.parent);
    }

    public void StartGame()
    {
        levelText.text = $"{level}";
        if (letPositions == null) letPositions = new List<Vector2>();
        else letPositions.Clear();
        if (letDict == null) letDict = new Dictionary<Vector2, Letter>();
        else letDict.Clear();
        if (!player.gameObject.activeSelf) player.gameObject.SetActive(true);
        player.GetComponent<Player>().hitPlayer = 3;
        
        GameObject c;
        Vector2 cell = Vector2.zero;
        for (int i = 0; i < init.lets.Count; i++) 
        {
            float v = init.lets.Count/2;
            cell = new Vector2(.5f - v + i, -4.5f);
            c = Instantiate(cellPrefab, cell, Quaternion.identity, cellAnchor);
            c.GetComponent<SpriteRenderer>().sprite = cells[0];
            letPositions.Add(cell);
        }
        c = Instantiate(cellPrefab, new Vector2(letPositions[0].x - 2f, letPositions[0].y), Quaternion.identity, cellAnchor);
        c.GetComponent<SpriteRenderer>().sprite = cells[1];
        c = Instantiate(cellPrefab, new Vector2(letPositions[letPositions.Count - 1].x + 2f, letPositions[letPositions.Count - 1].y), Quaternion.identity, cellAnchor);
        c.GetComponent<SpriteRenderer>().sprite = cells[2];
        phase = GamePhase.game;
    }
    
    private void CompleteGame()
    {
        phase = GamePhase.pause;
        int numValues = 0;
        
        for (int i = 0; i < letPositions.Count; i++)
            if (letDict.TryGetValue(letPositions[i], out Letter l) && l.charLet == init.lets[i].charLet)
                numValues++;
        if (numValues == init.lets.Count) Win();
        else Lose();
    }

    private void Win()
    {
        foreach (Transform child in cellAnchor) Destroy(child.gameObject);
        level++;
        levelText.text = $"{level}";
    }    
    
    private void Lose()
    {
        foreach (Transform child in cellAnchor) Destroy(child.gameObject);
        level--;
        levelText.text = $"{level}";
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
        if (phase != GamePhase.pause && letDict != null && letDict.Count == init.lets.Count) CompleteGame();
        
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
