using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Player : MonoBehaviour
{
    public float hitPlayer { private set; get; }
    public float maxHit;
    public float attack_speed { private set; get; }
    public float start_speed;
    public float attack_damage { private set; get; }
    public float start_damage;

    public int leaves_count;
    
    private TMP_Text txtLeaves, txtHp;
    private Image leaveImg, hpImg;
    
    private Vector2 startPos;
    private Color colorPlayer;

    void Start()
    {
        colorPlayer = GetComponentInChildren<SpriteRenderer>().color;
        
        hpImg = GetComponentsInChildren<Image>()[1];
        txtHp = GetComponentsInChildren<TMP_Text>()[0];
        leaveImg = GetComponentsInChildren<Image>()[3];
        txtLeaves = GetComponentsInChildren<TMP_Text>()[1];
        
        leaveImg.gameObject.SetActive(false);
        startPos = transform.position;
        
        SetHit( maxHit );
        SetSpeed( start_speed );
        SetDamage( start_damage );
    }

    public void SetHit( float hit )
    {
        hitPlayer = hit;
        txtHp.text = $"{hitPlayer}";
        hpImg.fillAmount = hitPlayer / maxHit;
        if( hitPlayer <= 0 ) GameBase.G.CompleteGame(); 
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
    
    public void SetLeavesCount()
    {
        leaves_count--;
        txtLeaves.text = $"{leaves_count}";
        if( leaves_count <= 0 ) leaveImg.gameObject.SetActive(false);
    }
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Enemy" )
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color( colorPlayer.r, colorPlayer.g - .5f, colorPlayer.b - .5f, colorPlayer.a );
            Damage( collision.GetComponent<Enemy>().attack );
        }

        if( collision.gameObject.tag == "Teleport" )
        {
            gameObject.SetActive(false);
            SetPos( startPos );
        }
        
        if( collision.gameObject.tag == "Leaves" ) {
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
}
