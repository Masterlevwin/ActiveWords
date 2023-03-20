using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    
    public static int level = 0;
    public TMP_Text levelText;
    
    public int coins_count = 0;
    public TMP_Text coinText;
    
    public AIPath player, enemy;
    public GameObject leavePrefab, coinPrefab;

    public Init init;
    private Dictionary<Vector2, Letter> letDict; 

    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);

        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        levelText.text = $"{level}";
        if (letDict == null) letDict = new Dictionary<Vector2, Letter>();
        else letDict.Clear();
        if (!player.gameObject.activeSelf) player.gameObject.SetActive(true);
        if (!enemy.gameObject.activeSelf) enemy.gameObject.SetActive(true);
        player.GetComponent<Player>().SetHit(player.GetComponent<Player>().maxHit);
        enemy.GetComponent<Player>().SetHit(enemy.GetComponent<Player>().maxHit);
        phase = GamePhase.game;
    }
    
    public void CompleteGame()
    {
        phase = GamePhase.complete;
        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);
        
        int numValues = 0;
        for (int i = 0; i < init.letPositions.Count; i++)
            if (letDict.TryGetValue(init.letPositions[i], out Letter l) && l.charLet == init.lets[i].charLet)
                numValues++;
        if (numValues == init.lets.Count) Win();
        else Lose();
    }

    private void Win()
    {
        level++;
        levelText.text = $"{level}";
    }    
    
    private void Lose()
    {
        level--;
        levelText.text = $"{level}";
    }
    
    public void RemoveAtWord( Letter l )
    {
        if( letDict.ContainsValue(l) )
        {
            letDict.Remove( l.transform.position );
            StartCoroutine( Move( l.gameObject, l.posLet, 4f, () => { l.GetComponent<BoxCollider2D>().isTrigger = true; CoinCreate( l.gameObject, -10 ); } ) );
        }
    } 
    
    public void AddToWord( Letter l )
    {
        for( int i = 0; i < init.letPositions.Count; i++ )
        { 
            if( !letDict.ContainsValue(l) && !letDict.ContainsKey(init.letPositions[i]) )
            {
                letDict.Add( init.letPositions[i], l );
                CoinCreate( l.gameObject, 10 );
                StartCoroutine( Move( l.gameObject, init.letPositions[i], 4f, () => { l.GetComponent<BoxCollider2D>().isTrigger = false; } ) );
                break;
            }
        }
    }
    
    public void LeaveStart( Vector2 target )
    {
        Player pl = player.GetComponent<Player>();
        GameObject leave = Instantiate( leavePrefab, player.transform.position, Quaternion.identity );
        leave.damage = pl.attack_damage;
        pl.SetLeavesCount();
        StartCoroutine( Move( pl.gameObject, target, pl.attack_speed, () => { Destroy( leave ); } ) );
    }

    public void CoinCreate( GameObject go, int price )
    {
        GameObject coin = Instantiate( coinPrefab, go.transform.position, Quaternion.identity, transform.parent );
        StartCoroutine( Move( coin, coinText.gameObject.transform.position, 3f, () => { coins_count += price; Destroy( coin ); } ) );
    }
    
    private IEnumerator Move( GameObject go, Vector2 endPosition, float speed = 1f, Action action = null )
    {
        float step = speed * Time.deltaTime;
        while( Vector2.Distance( go.transform.position, endPosition ) > float.Epsilon )
        {
            go.transform.position = Vector2.MoveTowards( go.transform.position, endPosition, step );
            yield return null;
        }
        action?.Invoke();
    }
    
    void Update()
    {
        coinText.text = $"{coins_count}";
        
        if (phase == GamePhase.game)
        {
            if (init.lets.Count != 0 && letDict.Count == init.lets.Count ) CompleteGame();
            player.canMove = true;
            enemy.canMove = true;
        } 
        else
        {
            player.canMove = false;
            enemy.canMove = false;
        }
    }
}
