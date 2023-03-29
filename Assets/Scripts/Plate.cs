using UnityEngine;

public class Plate : MonoBehaviour
{
  private bool isMove = false;
  
  void OnTriggerEnter2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      isMove = true;
    }
  }
  
  void Update()
  {
    if( isMove ) {
      transform.position = Vector2.MoveTowards( transform.position, transform.position + Vector2.right, time.deltaTime );
    }
  }
}
