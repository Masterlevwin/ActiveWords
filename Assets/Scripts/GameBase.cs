using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;
using UnityEngine.SceneManagement;

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
    
    public static int highscore = 0;
    public static int level = 0;
    public static string status = "Дошкольник";
    public int coins_count = 0;
    public TMP_Text highscoreText, levelText, coinText;
    public Image continueArea, levelUP, gameOver;
    public Canvas mainCanvas;
    public AIPath player, enemy;
    public GameObject leavePrefab, coinPrefab, trainingPrefab;
    public FlyDamage damagePrefab;
    public Player pl;
    public Enemy en;
    public TimerEnemyRebirth _timer;
    private Vector2 _startTimerPosition;
    private Init init;
    public Dictionary<Vector2, Letter> letDict;
    
    void Start()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);

        init = GetComponent<Init>();
        pl = player.GetComponent<Player>();
        en = enemy.GetComponent<Enemy>();
        en.DiedEnemy += CoinCreate;
        player.gameObject.SetActive(false);
        enemy.gameObject.SetActive(false);

        _startTimerPosition = _timer.transform.position;
        
        if( PlayerPrefs.HasKey( "SavedLevel" ) ) 
        {
            highscore = PlayerPrefs.GetInt( "SavedLevel" );
            highscoreText.text = $"{ highscore }";
        }
    }

    public void StartGame()
    {
        levelText.text = $"{level}";
        if (letDict == null) letDict = new Dictionary<Vector2, Letter>();
        else letDict.Clear();
        
        if (!player.gameObject.activeSelf) Waiter.Wait( 1f, () => { player.gameObject.SetActive(true); pl.SetPos( init.Spawn() ); } );
        if (level > 4 && !enemy.gameObject.activeSelf) Waiter.Wait( 2f, () => { enemy.gameObject.SetActive(true); en.transform.position = init.Spawn(); } );
        pl.maxHit = pl.hitPlayer;
        
        _timer.BeginTimer( _startTimerPosition, 4f );
        Waiter.Wait( 4f, () => { phase = GamePhase.game; trainingPrefab.SetActive(false); SoundManager.PlaySound("MissCloud"); } );
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
        SoundManager.PlaySound("FinishWork");
        level++;
        levelText.text = $"{level}";
        
        if( highscore < level ) 
        {
            highscore = level;
            PlayerPrefs.SetInt( "SavedLevel", highscore );
            PlayerPrefs.Save();
            highscoreText.text = $"{ highscore }";
        }
        
        if( level > 5 ) levelUP.gameObject.SetActive(true);
        else continueArea.gameObject.SetActive(true);
        pl.SetHit( pl.maxHit );
        en.SetHit( en.max_health - en.health );
    }    
    
    private void Lose()
    {
        SoundManager.PlaySound("LoseLevel");
        level--;
        if (level < 0) level = 0;
        levelText.text = $"{level}";
        gameOver.gameObject.SetActive(true);
         
        if( highscore < level ) 
        {
            highscore = level;
            PlayerPrefs.SetInt( "SavedLevel", highscore );
            PlayerPrefs.Save();
            highscoreText.text = $"{ highscore }";
            gameOver.gameObject.GetComponentInChildren<TMP_Text>().text = $"Вы достигли {level} уровня и установили новый рекорд!/nВы - {Status()}";
        }
        else gameOver.gameObject.GetComponentInChildren<TMP_Text>().text = $"Вы достигли {level} уровня, но не превзошли свой рекорд./nВы - {Status()}";
        
        player.maxSpeed = 5;
        pl.ResetProperties();
        pl.SetLeavesCount( pl.leaves_count );
        enemy.maxSpeed = 2;
        en.ResetProperties();
        coins_count = 0;
    }

    public void RestartLevel()
    {
        if (letDict.Count != init.lets.Count)
        {
            coins_count -= 10;
            if (coins_count < 0) coins_count = 0;
        }
        init.Reset();
    }

    public void NewGame()
    {
        highscore = 0;
        PlayerPrefs.SetInt( "SavedLevel", highscore );
        PlayerPrefs.Save();
        highscoreText.text = $"{ highscore }";
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
    
    public void TrainingView()
    {
        if (level == 0) TextView("Буквы выскочили из книги. Собери их обратно в слово!");
        if (level == 1) TextView("Буквы могут быть спрятаны в листве, за стволами деревьев или камнями");
        if (level == 2) TextView("Если схватил неверную букву, нажми на неё, чтобы она вернулась на поле");
        if (level == 3) TextView("Иногда некоторые слова придётся определить по их описанию");
        if (level == 4) TextView("Некоторые буквы лежат на поленьях, которые долго не выдержат вес целой книги");
        if (level == 5) TextView("Надоедливую птицу можно прогнать листиками, выстреливая по ней двойным кликом");
    }

    public void TextView( string txt )
    {
        trainingPrefab.SetActive(true);
        trainingPrefab.GetComponentInChildren<TMP_Text>().text = txt;
    }
    
    private string Status()
    {
        switch ( level )
        {
            default:
                status = $"Философ";
                break;
            case < 3:
                status = $"Дошкольник";
                break;
            case < 6:
                status = $"Ученик";
                break;
            case < 9:
                status = $"Студент";
                break;
            case < 12:
                status = $"Мудрец";
                break;
        }
        return status;
    }
    
    public void RemoveAtWord( Letter l )
    {
        if ( letDict.ContainsValue(l) )
        {
            letDict.Remove( l.transform.position );
            l.transform.position = l.posLet;
            coins_count--;
        }
    }
    
    public void AddToWord( Letter l )
    {
        for( int i = 0; i < init.letPositions.Count; i++ )
        {
            Vector2 pos = init.letPositions[i];
            if( !letDict.ContainsValue(l) && !letDict.ContainsKey(pos) )
            {
                letDict.Add( pos, l );
                CoinCreate( l.gameObject, 1 );
                StartCoroutine( Move( l.gameObject, pos, 4f, () => { if (l.charLet == init.lets[i].charLet) l._inWord = true; } ) );
                break;
            }
        }
        if( init.lets.Count != 0 && letDict.Count == init.lets.Count ) Waiter.Wait( .3f, () => { CompleteGame(); } );
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
        else StartCoroutine ( Move( go, go.transform.position + Vector3.down, .3f ) );
    }

    public void FlyDamage( GameObject go, float damage )
    {
        FlyDamage _flyDamage = Instantiate( damagePrefab, go.transform.position, Quaternion.identity, mainCanvas.transform );      
        _flyDamage.SetDamage( Mathf.Abs( damage ) );
    }
    
    public List<GameObject> gOs = new ();
    private IEnumerator Move( GameObject go, Vector2 endPosition, float speed = 1f, Action action = null )
    {
        if( !go.GetComponent<Letter>() ) gOs.Add( go );
        float step = speed * Time.deltaTime;
        while( Vector2.Distance( go.transform.position, endPosition ) > float.Epsilon )
        {
            go.transform.position = Vector2.MoveTowards( go.transform.position, endPosition, step );
            yield return null;
        }
        if( !go.GetComponent<Letter>() ) gOs.Remove( go );
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
