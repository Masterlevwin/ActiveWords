using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : MonoBehaviour
{
  void OnTriggerEnter2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Enemy" ) {
      Destroy( gameObject );
    }
  }
}
