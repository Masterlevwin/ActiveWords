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
    private TMP_Text txtAtt;
    
    private Vector2 startPos;
    private Action<float>[] actions;
    
    public event Action<GameObject, int, float> DiedEnemy;
    
    void Start()
    {
        hpImg = GetComponentsInChildren<Image>()[1];
        txtHp = GetComponentsInChildren<TMP_Text>()[0];
        txtAtt = GetComponentsInChildren<TMP_Text>()[1];
        startPos = transform.position;
        ResetProperties();
        actions = new Action<float>[] { SetAttack, SetRebirth, SetSpeed } ;
    }
    
    public void ResetProperties()
    {
        transform.localScale = Vector3.one;
        health = 0;
        attack = 0;
        rebirth = 0;
        GameBase.G.enemy.maxSpeed = 2;
        max_health = start_health;
        SetHit( start_health );
        SetAttack( start_attack );
        SetRebirth( -start_rebirth );
    }

    private void Died()
    {
        DiedEnemy?.Invoke( gameObject, 10, rebirth );
        gameObject.SetActive(false);
        SetHit( ++max_health );
        max_health = health;
        actions[ UnityEngine.Random.Range( 0, actions.Length ) ]( 1f );
        Waiter.Wait( rebirth, () =>
        {
            if( GameBase.G.phase == GamePhase.game )
            {
                gameObject.SetActive(true);
                GameBase.G.enemy.SearchPath();
                SoundManager.PlaySound( "RebirthEnemy" );
            }
        });
    }
    
    public void Damage( float _damage )
    {
        health -= _damage;
        SoundManager.PlaySound("BirdPunch");
        GameBase.G.FlyDamage( gameObject, GameBase.G.pl.attack_damage );
        SetHit( health );
    }
    
    public void SetHit( float _hp )
    {
        health = _hp;
        txtHp.text = $"{health}";
        hpImg.fillAmount = health / max_health;
        if( health <= 0 ) Died();
    }
    
    public void SetAttack( float _attack )
    {
        attack += _attack;
        txtAtt.text = $"{attack}";
    }
    
    public void SetRebirth( float _rebirth )
    {
        rebirth -= _rebirth;
        if( rebirth <= 0 ) rebirth = 1f;
    }
    
    public void SetSpeed( float _speed )
    {
        GameBase.G.enemy.maxSpeed += _speed;
        transform.localScale *= 1.1f;
    }

    public void SetPos( Vector2 pos )
    {
        if( pos == startPos && !gameObject.activeSelf ) gameObject.SetActive(true);
        transform.position = pos;
        if (GameBase.G.phase == GamePhase.game) SoundManager.PlaySound( "Magic Spell_Short Reverse_1" );
    }
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Player" )
        {
            GameBase.G.pl.Damage( attack );
            SetPos( startPos );
        }

        if ( collision.gameObject.tag == "Leave" ) {
            Damage( GameBase.G.pl.attack_damage );
            collision.gameObject.SetActive( false );
            GameBase.G._leaveActive = false;
        }
    }
}
