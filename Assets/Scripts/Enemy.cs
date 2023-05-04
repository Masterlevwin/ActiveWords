using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

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
    
    public Transform startPoint;
    //private Vector2 startPos;
    private Action<float>[] actions;
    
    public event Action<GameObject, int, float> DiedEnemy;
    
    void Start()
    {
        hpImg = GetComponentsInChildren<Image>()[1];
        txtHp = GetComponentsInChildren<TMP_Text>()[0];
        txtAtt = GetComponentsInChildren<TMP_Text>()[1];

        //startPos = startPoint.position;
        ResetProperties();
        actions = new Action<float>[] { SetAttack, SetRebirth, SetMaxHealth } ;
    }
    
    public void ResetProperties()
    {
        transform.localScale = Vector3.one;
        health = 0;
        attack = 0;
        rebirth = 0;
        GetComponent<AIPath>().maxSpeed = 2;
        max_health = start_health;
        SetHit( start_health );
        SetAttack( start_attack );
        SetRebirth( -start_rebirth );
    }

    private void Died()
    {
        DiedEnemy?.Invoke( gameObject, 10, rebirth );
        gameObject.SetActive(false);
        SetHit( max_health );
        actions[ UnityEngine.Random.Range( 0, actions.Length ) ]( 1f );
        Waiter.Wait( rebirth, () =>
        {
            if( GameBase.G.phase == GamePhase.game )
            {
                SoundManager.PlaySound( "RebirthEnemy" );
                gameObject.SetActive(true);
                GameBase.G.enemy.SearchPath();
            }
        });
    }
    
    public void Damage( float _damage )
    {
        health -= _damage;
        SoundManager.PlaySound( "BirdPunch" );
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
    
    public void SetMaxHealth( float _max )
    {
        max_health += _max;
        SetHit( max_health );
    }
    
    public void SetSpeed( float _speed )
    {
        GameBase.G.enemy.maxSpeed += _speed;
        if( GameBase.G.enemy.maxSpeed == GameBase.G.player.maxSpeed ) GameBase.G.enemy.maxSpeed--;
        else transform.localScale *= 1.1f;
    }

    /*public void SetPos( Vector2 pos )
    {
        if( pos == startPos && !gameObject.activeSelf ) gameObject.SetActive(true);
        transform.position = pos;
        if (GameBase.G.phase == GamePhase.game) SoundManager.PlaySound( "Magic Spell_Short Reverse_1" );
    }*/
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Player" )
        {
            GameBase.G.pl.Boom( attack );
            //SetPos( startPos );  
            //StartCoroutine( MoveToStartPosition() );
            GetComponent<AIDestinationSetter>().target = startPoint;
        }

        if( collision.gameObject.tag == "Leave" )
        {
            Damage( GameBase.G.pl.attack_damage );
            collision.gameObject.SetActive( false );
            GameBase.G._leaveActive = false;
        }
        
        if( collision.transform == startPoint )
        {
            SoundManager.PlaySound( "Magic Spell_Short Reverse_1" );
            GetComponent<AIDestinationSetter>().target = GameBase.G.pl.transform;
        }
    }
    
    /*private IEnumerator MoveToStartPosition()
    {
        AIPath ai = GetComponent<AIPath>();
        ai.destination = startPos;
        ai.SearchPath();
        while( !ai.reachedDestination ) {
            yield return null;
        }
    }*/
}
