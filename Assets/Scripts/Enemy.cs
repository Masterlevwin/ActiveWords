using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Enemy: MonoBehaviour
{
    public float health { private set; get; }
    public float max_health;
    public float attack { private set; get; }
    public float start_attack;
    public float rebirth { private set; get; }
    public float time_rebirth;
    
    private Image hpImg;
    private TMP_Text txtHp;
    
    private Vector2 startPos;
    private System.Action<float>[] actions;
    
    void Start()
    {
        hpImg = GetComponentsInChildren<Image>()[1];
        txtHp = GetComponentsInChildren<TMP_Text>()[0];
        
        startPos = transform.position;
        SetHit( max_health );
        SetAttack( start_attack );
        SetRebirth( -time_rebirth );
        
        actions = new System.Action<float>[] { SetHit, SetAttack, SetRebirth } ;
    }
    
    public void SetHit( float _hp )
    {
        health += _hp;
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
            if( GameBase.G.phase == GamePhase.game ) gameObject.SetActive(true);
            actions[ Random.Range( 0, actions.Length ) ]( 1f );
        });
    }
    
    public void SetAttack( float _attack )
    {
        attack += _attack;
        if( attack <= 0 ) attack = 1f;
    }
    
    public void SetRebirth( float _rebirth )
    {
        rebirth -= _rebirth;
        if( rebirth <= 0 ) rebirth = 1f;
    }
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Leave" ) {
            SetHit( -GameBase.G.pl.attack_damage );
            collision.gameObject.SetActive( false );
            GameBase.G._leaveActive = false;
        }
    }
}
