using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    private TMP_Text txtHp;
    private Image bar;
    
    private Vector2 startPos;
    private Color colorPlayer;

    void Start()
    {
        colorPlayer = GetComponentInChildren<SpriteRenderer>().color;
        txtHp = GetComponentInChildren<TMP_Text>();
        bar = GetComponentsInChildren<Image>()[1];
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
            else 
        } 
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
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Enemy" )
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color( colorPlayer.r, colorPlayer.g - .5f, colorPlayer.b - .5f, colorPlayer.a );
            Damage(1);
        } 
        
        if (collision.gameObject.tag == "Teleport")
        {
            gameObject.SetActive(false);
            SetPos(startPos);
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
        if( !is_player && GameBase.G.phase == GamePhase.game ) StartCoroutine( Shot(this) );
    }
}
