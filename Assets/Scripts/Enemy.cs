using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy: MonoBehaviour
{
    public float health { private set; get; }
    public float start_health;
    public float max_health;
    public float attack { private set; get; }
    public float start_attack;
    public float rebirth { private set; get; }
    public float start_rebirth;
    
    private Image hpImg;
    private TMP_Text txtHp;

    private Vector2 startPos;
    private Action<float>[] actions;
    
    public event Action<GameObject, int, float> DiedEnemy;
    
    void Start()
    {
        hpImg = GetComponentsInChildren<Image>()[1];
        txtHp = GetComponentsInChildren<TMP_Text>()[0];
        
        startPos = transform.position;
        ResetProperties();
        actions = new Action<float>[] { SetHit, SetAttack, SetRebirth, SetSpeed } ;
    }
    
    public void ResetProperties()
    {
        health = 0;
        attack = 0;
        rebirth = 0;
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
        DiedEnemy?.Invoke( gameObject, 10, rebirth );
        gameObject.SetActive(false);
        SetHit( max_health );

        Waiter.Wait( rebirth, () =>
        {
            if( GameBase.G.phase == GamePhase.game ) {
                actions[ UnityEngine.Random.Range( 0, actions.Length ) ]( 2f );
                max_health = health;
                gameObject.SetActive(true);
                GameBase.G.enemy.SearchPath();
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
    
    public void SetSpeed( float _speed )
    {
        GameBase.G.enemy.maxSpeed += _speed - 1f;
        if( GameBase.G.enemy.maxSpeed <= 0 ) GameBase.G.enemy.maxSpeed = 1f;
    }

    public void SetPos(Vector2 pos)
    {
        if (pos == startPos && !gameObject.activeSelf) gameObject.SetActive(true);
        transform.position = pos;
    }
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if (collision.gameObject.tag == "Player" )
        {
            GameBase.G.pl.Damage( attack );
            SetPos( startPos );
        }

        if ( collision.gameObject.tag == "Leave" ) {
            SetHit( -GameBase.G.pl.attack_damage );
            collision.gameObject.SetActive( false );
            GameBase.G._leaveActive = false;
        }
    }
}