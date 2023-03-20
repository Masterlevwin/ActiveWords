using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : MonoBehaviour
{
    public float damage = 1f;
    
    void OnTriggerEnter2D( Collider2D collision )
    {
        if( collision.gameObject.tag == "Enemy" ) {
            Destroy( gameObject );
        }
    }
}