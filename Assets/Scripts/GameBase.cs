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

        pl = player.GetComponent<Player>();
        en = enemy.GetComponent<Enemy>();
        en.DiedEnemy += CoinCreate;
        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        levelText.text = $"{level}";
        //ModeEnemy();
        if (letDict == null) letDict = new Dictionary<Vector2, Letter>();
        else letDict.Clear();
        if (!player.gameObject.activeSelf) Waiter.Wait( 1f, () => { player.gameObject.SetActive(true); pl.SetPos( init.Spawn() ); } );
        if (!enemy.gameObject.activeSelf) Waiter.Wait( 2f, () => { enemy.gameObject.SetActive(true); en.transform.position = init.Spawn(); } );
        pl.maxHit = pl.hitPlayer;
        Waiter.Wait( 3f, () => { phase = GamePhase.game; } );
    }
    
    public void ModeEnemy()
    {
        if (level < 10) enemy.repathRate = 4f;
        else if (level >= 10 && level < 15) enemy.repathRate = 2f;
        else enemy.repathRate = .5f;
    }
    
    public void CompleteGame()
    {
        StopMove();
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
        init.ClearLetters();
    }

    private void Win()
    {
        level++;
        levelText.text = $"{level}";
        levelUP.gameObject.SetActive(true);
        pl.SetHit( pl.maxHit );
    }    
    
    private void Lose()
    {
        level--;
        levelText.text = $"{level}";
        gameOver.gameObject.SetActive(true);
        pl.ResetProperties();
        pl.SetLeavesCount( pl.leaves_count );
        en.ResetProperties();
        coins_count = 0;
    }
    
    public void UpgradeAbility( int ability )
    {
        if( ability == 1 && coins_count >= 25 ) {
            pl.SetDamage( 1 );
            coins_count -= 25;
        }
        if( ability == 2 && coins_count >= 50 ) {
            pl.SetHit( ++pl.maxHit );
            coins_count -= 50;
        }
        if( ability == 3 && coins_count >= 70 ) {
            player.maxSpeed++;
            coins_count -= 70;
        }
        if( ability == 4 && coins_count >= 150 ) {
            pl.SetSpeed( 1 );
            coins_count -= 150;
        }
        init.Reset();
    }

    public void RestartScene( string scene )
    {
        SceneManager.LoadScene( scene );
    }

    public void RemoveAtWord( Letter l )
    {
        if( letDict.ContainsValue(l) )
        {
            pl.SetPos( pl.startPos );
            letDict.Remove( l.transform.position );
            _moveRoutine = StartCoroutine( Move( l.gameObject, l.posLet, 4f, () => { l.GetComponent<BoxCollider2D>().isTrigger = true; CoinCreate( l.gameObject, -1 ); } ) );
        }
    } 
    
    public void AddToWord( Letter l )
    {
        for( int i = 0; i < init.letPositions.Count; i++ )
        { 
            if( !letDict.ContainsValue(l) && !letDict.ContainsKey(init.letPositions[i]) )
            {
                letDict.Add( init.letPositions[i], l );
                CoinCreate( l.gameObject, 1 );
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

    public void CoinCreate( GameObject go, int price, float rebirth = 1f )
    {
        if( go == en.gameObject ) _timer.BeginTimer( go.transform.position, rebirth );
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
