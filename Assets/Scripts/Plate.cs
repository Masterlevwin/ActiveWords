using UnityEngine;

public class Plate : MonoBehaviour
{
  private Vector2 startPos;
  
  void Start()
  {
    startPos = transform.position;
  }
  
  void OnTriggerEnter2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      GameBase.G.PlateMove( gameObject, true );
    }
  }

  void OnTriggerExit2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      GameBase.G.PlateMove( gameObject, false );
    }
  }
}
