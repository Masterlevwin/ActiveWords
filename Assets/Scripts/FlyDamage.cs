using UnityEngine;
using System.Collections;
using TMPro;

public class FlyDamage : MonoBehaviour
{
    private TMP_Text damageText;
    private Vector3 endPosition;
    
    private void OnEnable()
    {
        damageText = GetComponent<TMP_Text>();
        endPosition = transform.position + Vector3.up*2;
    }
    
    public void SetDamage( float damage )
    {
        damageText.text = $"{ damage }";
        StartCoroutine( Fly() );
    }

    private IEnumerator Fly()
    {
        while( endPosition.y - transform.position.y > .1f )
        {
            transform.position = Vector3.Slerp( transform.position, endPosition, .1f );
            yield return null;
        }
        StartCoroutine( Miss() );
    }

    private IEnumerator Miss()
    {
        while( damageText.alpha > .1f )
        {
            damageText.alpha = Mathf.Lerp( damageText.alpha, 0f, Time.deltaTime * 3 );
            yield return null;
        }
        Destroy( gameObject );
    }
}
