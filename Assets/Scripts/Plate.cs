using UnityEngine;

public class Plate : MonoBehaviour
{
  bool _isMove = false;
  public GameObject _trunk;

  private Vector2 _position;
  private Quaternion _rotation;
  private Vector2 _startPosition;
  private Vector2 _endPosition;
  private Quaternion _endRotation;

  void Start()
  {
    _position = _trunk.transform.position;
    _rotation = _trunk.transform.rotation;
    _startPosition = _trunk.transform.localPosition;
    _endPosition = new Vector2( _trunk.transform.position.x - 1.5f, _trunk.transform.position.y - 1f);
    _endRotation = Quaternion.Euler(0, 0, -90f);
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
            _trunk.transform.rotation = Quaternion.Slerp( _trunk.transform.rotation, _endRotation, Time.deltaTime );

            if( Vector2.Distance( _trunk.transform.localPosition, _startPosition ) >= 3f )
            {
                GameBase.G.pl.Boom(); 
                _isMove = false;
            }
        }
        else 
        {
            _trunk.transform.position = _position;
            _trunk.transform.rotation = _rotation;
        }
    }
}
