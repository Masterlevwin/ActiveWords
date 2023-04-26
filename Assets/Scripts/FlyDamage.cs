using UnityEngine;
using TMPro;

public class FlyDamage : MonoBehaviour
{
    private TMP_Text damageText;
    private float startTime;
    
    private void Start()
    {
        damageText = GetComponent<TMP_Text>();
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
