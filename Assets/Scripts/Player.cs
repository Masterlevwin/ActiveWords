using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Player : MonoBehaviour
{
    public float hitPlayer { private set; get; }
    public float start_hit;
    public float maxHit;
    public float attack_speed { private set; get; }
    public float start_speed;
    public float attack_damage { private set; get; }
    public float start_damage;
    public int leaves_count { private set; get; }
    
    private TMP_Text txtLeaves, txtHp;
    private Image leaveImg, hpImg;
    
    public Vector2 startPos;

    void Start()
    {        
        hpImg = GetComponentsInChildren<Image>()[1];
        txtHp = GetComponentsInChildren<TMP_Text>()[0];
        leaveImg = GetComponentsInChildren<Image>()[3];
        txtLeaves = GetComponentsInChildren<TMP_Text>()[1];
        
        leaveImg.gameObject.SetActive(false);
        ResetProperties();
    }
    
    public void ResetProperties()
    {
        hitPlayer = 0;
        attack_damage = 0;
        attack_speed = 0;
        startPos = transform.position;
        GetComponent<AIPath>().maxSpeed = 5;
        maxHit = start_hit;
        SetHit( start_hit );
        SetSpeed( start_speed );
        SetDamage( start_damage );
	    SetLeavesCount( leaves_count );
    }

    public void Damage( float dmg )
    {
        hitPlayer -= dmg;
        SoundManager.PlaySound( "Bloody punch" );
        GameBase.G.FlyDamage( gameObject, dmg );
        SetHit( hitPlayer );
    }
    
    public void SetHit( float hit )
    {
        hitPlayer = hit;
        if( hitPlayer >= maxHit ) maxHit = hitPlayer;
        txtHp.text = $"{hitPlayer}";
        hpImg.fillAmount = hitPlayer / maxHit;
        if( hitPlayer <= 0 ) GameBase.G.CompleteGame();   
    }
    
    public void SetSpeed( float speed )
    {
        attack_speed += speed;
    }
    
    public void SetDamage( float damage )
    {
        attack_damage += damage;
    }
    
    public void SetPos( Vector2 pos )
    {
        if( pos == startPos && !gameObject.activeSelf ) gameObject.SetActive(true);
        transform.position = pos;
    }
    
    public void Boom( float dmg = 1f )
    {
    	Damage( dmg );
        gameObject.SetActive(false);
        if ( GameBase.G.phase != GamePhase.complete ) {
	        GameBase.G._timer.BeginTimer( startPos, 3f );
            Waiter.Wait( 3f, () => { gameObject.SetActive(true); SetPos( startPos ); } );
	    }
    }

    public void SetLeavesCount( int lv = 1 )
    {
        leaves_count -= lv;
        txtLeaves.text = $"{leaves_count}";
        if( leaves_count <= 0 ) leaveImg.gameObject.SetActive(false);
	    else leaveImg.gameObject.SetActive(true);
    }
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Leaves" )
	{
            SoundManager.PlaySound( "DragLeaves" );
            Instantiate(GameBase.G.fx[0], transform.position, Quaternion.identity );
            SetLeavesCount( -10 );
            Destroy( collision.gameObject );
        }
    }
  
    public void OnGUI()
    {
        if ( Event.current.button == 0 && Event.current.clickCount == 2 && leaves_count > 0 && !GameBase.G._leaveActive ) {
			GameBase.G.LeaveStart( Camera.main.ScreenToWorldPoint(Input.mousePosition) );
		}
    }
}
