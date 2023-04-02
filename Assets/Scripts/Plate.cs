using UnityEngine;

public class Plate : MonoBehaviour
{
  private Vector2 startPos;
  bool _isMove = false;

  void Start()
  {
    startPos = transform.position;
  }
  
  void OnTriggerEnter2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      //GameBase.G.PlateMove( gameObject, true );
      _isMove = true;
    }
  }

  void OnTriggerExit2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      //GameBase.G.PlateMove( gameObject, false );
    }
  }

    private void Update()
    {
        if( _isMove )
        {
            Vector2 endPosition = new Vector2(transform.position.x, transform.position.y + 1f);
            transform.position = Vector2.MoveTowards( transform.position, endPosition, Time.deltaTime );
            if (Vector2.Distance(GameBase.G.pl.transform.position, endPosition) >= 5f)
            {
                GameBase.G.pl.SetHit(0);
                _isMove = false;
            }
        }
        else { transform.position = startPos; }
    }
}
