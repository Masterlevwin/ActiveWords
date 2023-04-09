using UnityEngine;

public class Plate : MonoBehaviour
{
  bool _isMove = false;
  public GameObject _trunk;
  private Vector2 _position;
  private Vector2 _startPosition;
  private Vector2 _endPosition;

  void Start()
  {
    _position = _trunk.transform.position;
    _startPosition = _trunk.transform.localPosition;
    _endPosition = new Vector2( _trunk.transform.position.x - 3f, _trunk.transform.position.y );
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
            _trunk.transform.position = Vector2.MoveTowards( _trunk.transform.position, _endPosition, Time.deltaTime );
            if (Vector2.Distance( _trunk.transform.localPosition, _startPosition) >= 3f )
            {
                GameBase.G.pl.SetHit(0);
                _isMove = false;
            }
        }
        else 
        {
            _trunk.transform.position = _position;
        }
    }
}
