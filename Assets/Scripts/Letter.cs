using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{ 
    public Vector3 posLet { private set; get; }
    public char charLet { private set; get; }

    private SpriteRenderer childRend;
    private void OnEnable()
    {
        SetLetterPos(transform.position);
        childRend = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    
    public void SetLetterPos(Vector2 pos)
    {
        posLet = pos;
    }
    
    public void SetChar(char l)
    {
        charLet = l;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameBase.G.phase == GamePhase.game && Vector2.Distance(GameBase.G.pl.transform.position, posLet) > 1f)
        {
            GameBase.G.RemoveAtWord(this);
            childRend.gameObject.SetActive(true);
        }
        else return;  // Отрисовка ошибочного нажатия - мигание posLet со звуком ошибки типа место занято
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"
            && Vector2.Distance(transform.position, GameBase.G.player.destination) < 1f)
        {
            GameBase.G.AddToWord(this);
            childRend.gameObject.SetActive(false);
        }
    }

    public bool _hasRotation = true;
    public float rotationSpeed = 1;
    public AnimationCurve rotationAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    void Update()
    {
        if ( _hasRotation )
        {
            childRend.transform.localEulerAngles = new Vector3( 0, 0, -360 * rotationAnimationCurve.Evaluate( ( rotationSpeed * Time.time ) % 1 ) );
        }
    }
}
