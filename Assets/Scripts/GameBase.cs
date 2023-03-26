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
    public Image levelUP;
    public Image gameOver;
    public int coins_count = 0;
    public TMP_Text coinText;
    
    public AIPath player, enemy;
    public GameObject leavePrefab, coinPrefab;
    public Player pl;
    public Enemy en;
    public TimerEnemyRebirth _timer;
    public Init init;
    private Dictionary<Vector2, Letter> letDict;
    
    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);

        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);
        
        pl = player.GetComponent<Player>();
        en = enemy.GetComponent<Enemy>();
    }

    public void StartGame()
    {
        levelText.text = $"{level}";
        if (letDict == null) letDict = new Dictionary<Vector2, Letter>();
        else letDict.Clear();
        if (!player.gameObject.activeSelf) Waiter.Wait( .5f, () => { player.gameObject.SetActive(true); } );
        if (!enemy.gameObject.activeSelf) Waiter.Wait( .5f, () => { enemy.gameObject.SetActive(true); } );
        pl.SetHit( pl.maxHit );
        en.ResetProperties();
        phase = GamePhase.game;
    }
    
    public void CompleteGame()
    {
        phase = GamePhase.complete;
        if ( _timer.gameObject.activeSelf ) _timer.StopTimer();
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
        levelUP.gameObject.SetActive(true);
    }    
    
    private void Lose()
    {
        level--;
        levelText.text = $"{level}";
        gameOver.gameObject.SetActive(true);
    }
    
    public void RemoveAtWord( Letter l )
    {
        if( letDict.ContainsValue(l) )
        {
            letDict.Remove( l.transform.position );
            _moveRoutine = StartCoroutine( Move( l.gameObject, l.posLet, 4f, () => { l.GetComponent<BoxCollider2D>().isTrigger = true; CoinCreate( l.gameObject, -10 ); } ) );
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
                _moveRoutine = StartCoroutine( Move( l.gameObject, init.letPositions[i], 4f, () => { l.GetComponent<BoxCollider2D>().isTrigger = false; } ) );
                break;
            }
        }
    }
    
    public bool _leaveActive = false;
    
    public void LeaveStart( Vector2 target )
    {
        GameObject leave = Instantiate( leavePrefab, pl.transform.position, Quaternion.identity );
        pl.SetLeavesCount( 1f );
        _leaveActive = true;
        _moveRoutine = StartCoroutine( Move( leave, target, pl.attack_speed, () => { Destroy( leave ); if( _leaveActive ) _leaveActive = false; } ) );
    }

    public void CoinCreate( GameObject go, int price )
    {
        GameObject coin = Instantiate( coinPrefab, go.transform.position, Quaternion.identity, transform.parent );
        _moveRoutine = StartCoroutine( Move( coin, coinText.gameObject.transform.position, 4f, () => { coins_count += price; Destroy( coin ); } ) );
    }
    
    Coroutine _moveRoutine;
    private IEnumerator Move( GameObject go, Vector2 endPosition, float speed = 1f, Action action = null )
    {
        float step = speed * Time.deltaTime;
        while( Vector2.Distance( go.transform.position, endPosition ) > float.Epsilon )
        {
            go.transform.position = Vector2.MoveTowards( go.transform.position, endPosition, step );
            yield return null;
        }
        action?.Invoke();
        _moveRoutine = null;
    }

    public void StopMove()
    {
        if( _moveRoutine != null ) {
            StopCoroutine( _moveRoutine );
            _moveRoutine = null;
        }
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
