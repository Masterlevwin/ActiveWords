using UnityEngine;

public class Plate : MonoBehaviour
{
  bool _isMove = false;
  private SpriteRenderer _spritePlate;
  private Vector2 startPos;
  private Vector2 _endPosition;
  private Vector2 _startPosition;

  void Start()
  {
    startPos = transform.position;
    _spritePlate = GetComponentInChildren<SpriteRenderer>();
    _startPosition = _spritePlate.transform.localPosition;
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
            _spritePlate.transform.position = Vector2.MoveTowards( _spritePlate.transform.position, _endPosition, Time.deltaTime );
            if (Vector2.Distance(_spritePlate.transform.localPosition, _startPosition) >= 3f)
            {
                GameBase.G.pl.SetHit(0);
                _isMove = false;
            }
        }
        else 
        {
            _spritePlate.transform.position = startPos;
        }
    }
}
