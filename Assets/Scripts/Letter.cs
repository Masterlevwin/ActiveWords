using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{ 
    public Vector3 posLet { private set; get; }
    public char charLet { private set; get; }

    private SpriteRenderer sRend, spinnerRend;
    private BoxCollider2D col;

    private void OnEnable()
    {
        SetLetterPos(transform.position);
        sRend = GetComponent<SpriteRenderer>();
        spinnerRend = transform.GetChild(0).GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }
    
    public void SetLetterPos(Vector2 pos)
    {
        posLet = pos;
    }
    
    public void SetChar(char l)
    {
        charLet = l;
    }

    public bool _inWord = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if( eventData.button == PointerEventData.InputButton.Right ) return;

        if (GameBase.G.phase == GamePhase.game && !_inWord
            && Vector2.Distance(GameBase.G.pl.transform.position, posLet) > 1f)
        {
            GameBase.G.RemoveAtWord(this);
        }
        else {
            SoundManager.PlaySound("Infect");
            return;  // Отрисовка ошибочного нажатия - мигание posLet со звуком ошибки типа место занято
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"
            && Vector2.Distance(transform.position, GameBase.G.player.destination) < 1f)
        {
            SoundManager.PlaySound("Click1");
            GameBase.G.AddToWord(this);
        }
    }

    public bool _hasRotation = true;
    public float rotationSpeed = 1;
    public AnimationCurve rotationAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    void Update()
    {
        if ( _hasRotation )
        {
            spinnerRend.transform.localEulerAngles = new Vector3( 0, 0, -360 * rotationAnimationCurve.Evaluate( ( rotationSpeed * Time.time ) % 1 ) );
        }

        if( _inWord )
        {
            sRend.color = Color.magenta;
            spinnerRend.gameObject.SetActive(false);
            col.isTrigger = false;
        }
        else
        {
            sRend.color = Color.white;
            spinnerRend.gameObject.SetActive(true);
            col.isTrigger = true;
        }
    }
}
