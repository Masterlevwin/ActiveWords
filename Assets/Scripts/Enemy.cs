using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Enemy: MonoBehaviour
{
    public float health { private set; get; };
    public float max_health;
    public float attack { private set; get; };
    public float start_attack;
    public float rebirth { private set; get; };
    public float time_rebirth;
    
    private Image hpImg;
    private TMP_Text txtHp;
    
    private Vector2 startPos;
    private System.Action[] actions;
    
    void Start()
    {
        hpImg = GetComponentsInChildren<Image>()[1];
        txtHp = GetComponentsInChildren<TMP_Text>()[0];
        
        startPos = transform.position;
        SetHit( max_health );
        SetAttack( start_attack );
        SetRebirth( time_rebirth );
        
        actions = new System.Action[] { SetHit, SetAttack, SetBirth };
    }
    
    public void Damage( float _damage )
    {
        health -= _damage;
        SetHit( health );
    }
    
    public void SetHit( float _hp = 5f )
    {
        health = _hp;
        txtHp.text = $"{health}";
        hpImg.fillAmount = health / max_health;
        if( health <= 0 ) Died(); 
    }
    
    private void Died()
    {
        GameBase.G.CoinCreate( this.gameObject, 15 );
        Waiter.Wait( .5f, () => { GameBase.G.CoinCreate( this.gameObject, 15 ); } );
        gameObject.SetActive(false);
        Waiter.Wait( rebirth, () =>
        {
            gameObject.SetActive(true);
            actions[ Random.Range( 0, actions.Length ) ]();
        });
    }
    
    public void SetAttack( float _attack = 2f )
    {
        if( _attack > 0 ) attack = _attack;
        else return;
    }
    
    public void SetRebirth( float _rebirth = 5f )
    {
        if( _rebirth > 0 ) rebirth = _rebirth;
        else return;
    }
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Leave" ) {
            Damage( collision.GetComponent<Leave>().damage );
            collision.gameObject.SetActive( false );
        }
    }
}
