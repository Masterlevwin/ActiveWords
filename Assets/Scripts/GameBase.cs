using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    
    public static int level = 0;
    public static string status = "Дошкольник";
    public int coins_count = 0;
    public TMP_Text levelText, coinText;
    public Image continueArea, levelUP, gameOver;

    public AIPath player, enemy;
    public GameObject leavePrefab, coinPrefab, trainingPrefab;
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
        if (letDict == null) letDict = new Dictionary<Vector2, Letter>();
        else letDict.Clear();
        
        if( level == 0 ) TextView( "Буквы выскочили из книги. Собери их обратно в слово!" );
        if( level == 1 ) TextView( "Буквы могут быть спрятаны в листве, за стволами деревьев или камнями" );
        if( level == 2 ) TextView( "Иногда некоторые слова придётся определить по их описанию" );
        if( level == 3 ) TextView( "Некоторые буквы лежат на поленьях, которые долго не выдержат вес целой книги" );
        if( level == 4 ) TextView( "Птицу, которая мешает собирать буквы, можно прогнать листиками, выстреливая по ней двойным кликом" );
        
        if (!player.gameObject.activeSelf) Waiter.Wait( 1f, () => { player.gameObject.SetActive(true); pl.SetPos( init.Spawn() ); } );
        if (level > 3 && !enemy.gameObject.activeSelf) Waiter.Wait( 2f, () => { enemy.gameObject.SetActive(true); en.transform.position = init.Spawn(); } );
        pl.maxHit = pl.hitPlayer;
        enemy.SetPath(null);
        
        _timer.BeginTimer( new Vector2( 10f, -4f ), 4f );
        Waiter.Wait( 4f, () => { phase = GamePhase.game; } );
    }
    
    public void CompleteGame()
    {
        phase = GamePhase.complete;
        if ( _timer.gameObject.activeSelf ) _timer.StopTimer();
        if ( player.gameObject.activeSelf ) player.gameObject.SetActive(false);
        if ( enemy.gameObject.activeSelf ) enemy.gameObject.SetActive(false);

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
        if( level > 4 ) levelUP.gameObject.SetActive(true);
        else continueArea.gameObject.SetActive(true);
        pl.SetHit( pl.maxHit );
        en.SetHit( en.max_health - en.health );
    }    
    
    private void Lose()
    {
        level--;
        levelText.text = $"{level}";
        gameOver.gameObject.SetActive(true);
        gameOver.gameObject.GetComponentInChildren<TMP_Text>().text = $"Вы достигли {level} уровня. Вы - {Status()}";
        pl.ResetProperties();
        pl.SetLeavesCount( pl.leaves_count );
        en.ResetProperties();
        coins_count = 0;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void TextView( string txt )
    {
        trainingPrefab.SetActive(true);
        trainingPrefab.GetComponentInChildren<TMP_Text>().text = txt;
    }
    
    private void Status()
    {
        if( level > 2 ) status = $"Ученик";
        if( level > 4 ) status = $"Студент";
        if( level > 6 ) status = $"Мудрец";
        else status = $"Философ";
    }
    
    public void RemoveAtWord( Letter l )
    {
        if( letDict.ContainsValue(l) )
        {
            letDict.Remove( l.transform.position );
            l.transform.position = l.posLet;
            l.GetComponent<BoxCollider2D>().isTrigger = true;
            l.GetComponent<SpriteRenderer>().color = Color.white;
            coins_count--;
        }
    } 
    
    public void AddToWord( Letter l )
    {
        Letter let = l;
        for( int i = 0; i < init.letPositions.Count; i++ )
        {
            Vector2 pos = init.letPositions[i];
            if( !letDict.ContainsValue(let) && !letDict.ContainsKey(pos) )
            {
                letDict.Add( pos, let );
                CoinCreate( let.gameObject, 1 );
                StartCoroutine( Move( let.gameObject, pos, 4f, () => { let.GetComponent<BoxCollider2D>().isTrigger = false; 
                    if ( letDict.TryGetValue(pos, out Letter l) && l.charLet == init.lets[i].charLet )
                    let.GetComponent<SpriteRenderer>().color = Color.cyan; } ) );
                break;
            }
        }
        if( init.lets.Count != 0 && letDict.Count == init.lets.Count ) Waiter.Wait( .5f, () => { CompleteGame(); });
    }

    public bool _leaveActive = false;
    
    public void LeaveStart( Vector2 target )
    {
        GameObject leave = Instantiate( leavePrefab, pl.transform.position, Quaternion.identity );
        pl.SetLeavesCount( 1f );
        _leaveActive = true;
        StartCoroutine( Move( leave, target, pl.attack_speed, () => { Destroy( leave ); if( _leaveActive ) _leaveActive = false; } ) );
    }

    public void CoinCreate( GameObject go, int price, float rebirth = 1f )
    {
        if( go == en.gameObject ) _timer.BeginTimer( go.transform.position, rebirth );
        GameObject coin = Instantiate( coinPrefab, go.transform.position, Quaternion.identity, transform.parent );
        StartCoroutine( Move( coin, coinText.gameObject.transform.position, 1f, () => { coins_count += price; Destroy( coin ); } ) );
    }
    
    public void PlateMove( GameObject go, bool b )
    {
        if ( b ) StartCoroutine( Move( go, go.transform.position + Vector3.up, .1f ) );
        else StartCoroutine(Move(go, go.transform.position + Vector3.down, .3f));
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
