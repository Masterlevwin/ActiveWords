using UnityEngine;
using TMPro;

public class FlyDamage : MonoBehaviour
{
    private TMP_Text damageText;
    private float startTime;
    
    private string damageView;
    private Vector3 start;
    
    public FlyDamage( string damage )
    {
        damageView = damage;
    }
    
    private void Start()
    {
        damageText = GetComponent<TMP_Text>();
        
        //damageText.text = damageView;
        //start = transform.position;
        //transform.position += Vector3.down;
        //StartCoroutine( DamageView() );
    }
    
    private IEnumerator DamageView()
    {
        while( transform.positiion.y < start.y - .05f )
        {
            transform.position = Vector3.Slerp( transform.position, start, .1f );
            yield return null;
        }
        transform.position = start;
        Destroy( gameObject );
    }
    
    public void SetDamage( float damage, GameObject go )
    {
        if (damageText != null) damageText.text = $"{damage}";
        else Debug.Log( damage );
        
        startTime = Time.time;
        Vector3 endPosition = go.transform.position + Vector3.up;

        StartCoroutine( Fly( endPosition ) );
        
        //end = go.transform.position + Vector3.up;
        //isFly = true;
    }
    
    private IEnumerator Fly( Vector3 endPos, float speed = 1f )
    {
        float step = (Time.time - startTime) / speed; 
        while( Vector2.Distance( transform.position, endPos ) > float.Epsilon )
        {
            transform.position = Vector2.Slerp( transform.position, endPos, step );
            yield return null;
        }
        Destroy( gameObject );
    }
    
    /*private bool isFly;
    private Vector3 end;
    private void Update()
    {
        while( isFly && Vector2.Distance( transform.position, end ) > float.Epsilon )
            transform.position = Vector2.Slerp( transform.position, end, Time.time - startTime );
    }*/
}
