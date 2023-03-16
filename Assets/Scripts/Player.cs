using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Player : MonoBehaviour, IPointerClickHandler
{
    public float hitPlayer { private set; get; }
    public float maxHit;
    public float attack_speed { private set; get; }
    public float start_speed;
    public float attack_damage { private set; get; }
    public float start_damage;
    
    public bool is_player;
    public byte leaves_count;
    
    private TMP_Text txtLeaves, txtHp;
    private Image leaveImg, hpImg;
    
    private Vector2 startPos;
    private Color colorPlayer;

    void Start()
    {
        EventManager.LeaveCreated += SetLeavesCount;
        
        colorPlayer = GetComponentInChildren<SpriteRenderer>().color;
        txtLeaves = GetComponentsInChildren<TMP_Text>()[0];
        txtHp = GetComponentsInChildren<TMP_Text>()[1];
        leaveImg = GetComponentsInChildren<Image>()[0];
        hpImg = GetComponentsInChildren<Image>()[1];
        
        startPos = transform.position;
        SetHit( maxHit );
        SetSpeed( start_speed );
        SetDamage( start_damage );
    }

    public void SetHit( float hit )
    {
        hitPlayer = hit;
        txtHp.text = $"{hitPlayer}";
        bar.fillAmount = hitPlayer / maxHit;
        if( hitPlayer <= 0 )
        {
            if( is_player ) GameBase.G.CompleteGame();
            else Died();
        } 
    }
    
    private void Died()
    {
        CoinCreate( this.gameObject, 30 );
        gameObject.SetActive(false);
        Waiter.Wait(3f, () =>
        {
            gameObject.SetActive(true);
            SetHit( ++maxHit );
        });
    }
    
    public void SetSpeed( float speed )
    {
        attack_speed = speed;
    }
    
    public void SetDamage( float damage )
    {
        attack_damage = damage;
    }
    
    public void Damage( float dmg )
    {
        hitPlayer -= dmg;
        SetHit( hitPlayer );
    }
    
    public void SetPos( Vector2 pos )
    {
        if( pos == startPos && !gameObject.activeSelf ) gameObject.SetActive(true);
        transform.position = pos;
    }
    
    private void SetLeavesCount()
    {
        leaves_count--;
        txtLeaves.text = $"{leaves_count};
        if( leaves_count <= 0 ) leaveImg.gameObject.SetActive(false);
    }
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Enemy" )
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color( colorPlayer.r, colorPlayer.g - .5f, colorPlayer.b - .5f, colorPlayer.a );
            Damage( collision.GetComponent<Player>().attack_damage );
        } 
        
        if( collision.gameObject.tag == "Teleport" )
        {
            gameObject.SetActive(false);
            SetPos( startPos );
        }
        
        if( is_player && collision.gameObject.tag == "Leaves" ) {
            if( !leaveImg.gameObject.activeSelf ) leaveImg.gameObject.SetActive(true);
            leaves_count += 10;
            Destroy( collision.gameObject );
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GetComponentInChildren<SpriteRenderer>().color = colorPlayer;
        } 
    }
    
    public void OnPointerClick( PointerEventData eventData )
    {
        if( GameBase.G.phase != GamePhase.game ) return;
        if( !is_player ) GameBase.G.LeaveShot( this );
    }
}
