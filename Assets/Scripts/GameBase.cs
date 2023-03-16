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
    
    public byte coins_count = 0;
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
    
    public void RemoveAtWord(Letter l)
    {
        if (letDict.ContainsValue(l))
        {
            SetPos( startPos );
            player.SetPath(null);
            letDict.Remove(l.transform.position);
            StartCoroutine(Move(l, l.posLet, true));
            CoinCreate( l.gameObject, -10 );
        }
    } 
    
    public void AddToWord(Letter l)
    {
        for (int i = 0; i < init.letPositions.Count; i++)
        { 
            if (!letDict.ContainsValue(l) && !letDict.ContainsKey(init.letPositions[i]))
            {
                letDict.Add(init.letPositions[i], l);
                StartCoroutine(Move(l, init.letPositions[i], false));
                CoinCreate( l.gameObject, 10 );
                break;
            }
        }
    }

    private IEnumerator Move( Letter l, Vector2 target, bool b )
    {
        float step = 4f * Time.deltaTime;
        while( Vector2.Distance( l.transform.position, target ) > float.Epsilon )
        {
            l.transform.position = Vector2.MoveTowards( l.transform.position, target, step );
            yield return null;
        }
        l.GetComponent<BoxCollider2D>().isTrigger = b;
    }
    
    public void LeaveShot( Player target )
    {
        if( player.GetComponent<Player>().leaves_count > 0 ) StartCoroutine( Shot( target ) );
    }
    
    private IEnumerator Shot( Player target )
    {
        float speed = player.GetComponent<Player>().attack_speed * Time.deltaTime;
        float damage = player.GetComponent<Player>().attack_damage;
        
        GameObject bullet = Instantiate( bulletPrefab, player.transform.position, Quaternion.identity, player.transform );
        while( bullet )
        {
            bullet.transform.position = Vector2.MoveTowards( bullet.transform.position, target.transform.position, speed );
            yield return null;
        }
        
        target.Damage( damage );
    }
    
    public void CoinCreate( GameObject go, byte price )
    {
        GameObject coin = Instantiate( coinPrefab, go.transform.position, Quaternion.identity, transform.parent );
        coins_count += price;
        StartCoroutine( CoinMove( coin ) );
        //StartCoroutine( ObjectMove( coin, coinText.gameObject, Destroy, 4f ) );
    }
    
    private IEnumerator CoinMove( GameObject coin )
    {
        float step = 4f * Time.deltaTime;
        while( Vector2.Distance( coin.transform.position, coinText.gameObject.transform.position ) > float.Epsilon )
        {
            coin.transform.position = Vector2.MoveTowards( coin.transform.position, coinText.gameObject.transform.position, step );
            yield return null;
        }
        Destroy( coin );
    }
    
    private IEnumerator ObjectMove( GameObject start, GameObject end, Action<GameObject> action, float speed = 1f )
    {
        float step = speed * Time.deltaTime;
        while( Vector2.Distance( start.transform.position, end.transform.position ) > float.Epsilon )
        {
            start.transform.position = Vector2.MoveTowards( start.transform.position, end.transform.position, step );
            yield return null;
        }
        action( start );
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
