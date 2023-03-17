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
    public GameObject bulletPrefab, coinPrefab;

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
            player.SetPath(null);
            CoinCreate( l.gameObject, -10 );
            letDict.Remove( l.transform.position );
            StartCoroutine( SMove( l, l.posLet, true) );
            //StartCoroutine( Move( l.gameObject, l.posLet, LetterEnd( l, true ), 5f ) );
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
                StartCoroutine( SMove( l, init.letPositions[i], false ) );
                //StartCoroutine( Move( l.gameObject, init.letPositions[i], LetterEnd( l, false ), 5f ) );
                break;
            }
        }
    }

    private IEnumerator SMove( Letter l, Vector2 target, bool b )
    {
        float step = 5f * Time.deltaTime;
        while( Vector2.Distance( l.transform.position, target ) > float.Epsilon )
        {
            l.transform.position = Vector2.MoveTowards( l.transform.position, target, step );
            yield return null;
        }
        l.GetComponent<BoxCollider2D>().isTrigger = b;
    }
    
    private void LetterEnd( Letter l, bool b )
    {
        if( b == true ) letDict.Remove( l.transform.position );
        l.GetComponent<BoxCollider2D>().isTrigger = b;
    }
    
    public void LeaveShot( Player target )
    {
        if( player.GetComponent<Player>().leaves_count > 0 ) {
            float speed = player.GetComponent<Player>().attack_speed * Time.deltaTime;
            float damage = player.GetComponent<Player>().attack_damage;
            GameObject bullet = Instantiate( bulletPrefab, player.transform.position, Quaternion.identity, player.transform );
            StartCoroutine( Shot( target, bullet, speed, damage ) );
            //StartCoroutine( Move( bullet, target.transform.position, LeaveEnd( target, damage ), speed ) );
        }
    }
    
    private IEnumerator Shot( Player target, GameObject bullet, float speed, float damage )
    { 
        while( bullet )
        {
            bullet.transform.position = Vector2.MoveTowards( bullet.transform.position, target.transform.position, speed );
            yield return null;
        }      
        target.Damage( damage );
    }
    
    private void LeaveEnd( Player target, float damage )
    {
        target.Damage( damage );
    }
    
    public void CoinCreate( GameObject go, int price )
    {
        GameObject coin = Instantiate( coinPrefab, go.transform.position, Quaternion.identity, transform.parent );
        coins_count += price;
        //StartCoroutine( CoinMove( coin ) );
        StartCoroutine( Move( coin, coinText.gameObject.transform.position, Destroy( coin ), 3f ) );
    }
    
    private IEnumerator CoinMove( GameObject coin )
    {
        float step = 3f * Time.deltaTime;
        while( Vector2.Distance( coin.transform.position, coinText.gameObject.transform.position ) > float.Epsilon )
        {
            coin.transform.position = Vector2.MoveTowards( coin.transform.position, coinText.gameObject.transform.position, step );
            yield return null;
        }
        Destroy( coin );
    }
    
    private IEnumerator Move( GameObject go, Vector2 endPosition, Action action, float speed = 1f )
    {
        float step = speed * Time.deltaTime;
        while( Vector2.Distance( go.transform.position, endPosition ) > float.Epsilon || go)
        {
            go.transform.position = Vector2.MoveTowards( go.transform.position, endPosition, step );
            yield return null;
        }
        action;
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
