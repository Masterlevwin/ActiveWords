using UnityEngine;

public class Plate : MonoBehaviour
{
  private bool isMove = false;
  private Transform facePlate;
  
  void Start()
  {
    facePlate = transform.GetChild(0);
  }
  
  void OnTriggerEnter2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      isMove = true;
    }
  }
  
  void OnTriggerExit2D( Collider2D collision )
  {
    if( collision.gameObject.tag == "Player" ) {
      isMove = false;
    }
  }
  
  void Update()
  {
    if( isMove ) {
      facePlate.transform.position = Vector3.MoveTowards( facePlate.transform.position, facePlate.transform.position + Vector3.right, Time.deltaTime );
    }
  }
}
