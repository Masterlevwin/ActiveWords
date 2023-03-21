using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Enemy: MonoBehaviour
{
    public float health { private set; get; } = 0;
    public float start_health;
    public float max_health;
    public float attack { private set; get; } = 0;
    public float start_attack;
    public float rebirth { private set; get; } = 0;
    public float start_rebirth;
    
    private Image hpImg;
    private TMP_Text txtHp;
    
    private Vector2 startPos;
    private System.Action<float>[] actions;
    
    void Start()
    {
        hpImg = GetComponentsInChildren<Image>()[1];
        txtHp = GetComponentsInChildren<TMP_Text>()[0];
        
        actions = new System.Action<float>[] { SetHit, SetAttack, SetRebirth } ;
    }
    
    public void ResetProperties()
    {
        startPos = transform.position;
        max_health = start_health;
        SetHit( start_health );
        SetAttack( start_attack );
        SetRebirth( -start_rebirth );
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
        Waiter.Wait( .2f, () =>
        {
            GameBase.G.CoinCreate( this.gameObject, 15 );
            gameObject.SetActive(false);
        });
        
        SetHit( max_health );
        
        Waiter.Wait( rebirth, () =>
        {
            if( GameBase.G.phase != GamePhase.complete ) {
                gameObject.SetActive(true);
                actions[ Random.Range( 0, actions.Length ) ]( 1f );
                max_health = health;
            }
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
