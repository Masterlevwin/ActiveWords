using UnityEngine;

public class Plate : MonoBehaviour
{
  bool _isMove = false;
  private Transform _trunk;
  private Vector2 startPos;
  private Vector2 _endPosition;
  private Vector2 _startPosition;

  void Start()
  {
    startPos = transform.position;
    _trunk = transform.GetChild(0);
    _startPosition = _trunk.localPosition;
    _endPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + 3f);
  }
  
  void OnTriggerEnter2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      _isMove = true;
    }
  }

  void OnTriggerExit2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      _isMove = false;
    }
  }

    private void Update()
    {
        if( _isMove )
        {
            _trunk.transform.position = Vector2.MoveTowards( _trunk.position, _endPosition, Time.deltaTime );
            if (Vector2.Distance( _trunk.localPosition, _startPosition) >= 3f )
            {
                GameBase.G.pl.SetHit(0);
                _isMove = false;
            }
        }
        else 
        {
            _trunk.position = startPos;
        }
    }
}
