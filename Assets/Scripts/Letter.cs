using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{ 
    public Vector3 posLet { private set; get; }
    public char charLet { private set; get; }
    public byte priceLet { private set; get; }

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
        if (l == 'а') priceLet = 1;
        else if (l == 'б') priceLet = 3;
        else if (l == 'в') priceLet = 2;
        else if (l == 'г') priceLet = 3;
        else if (l == 'д') priceLet = 2;
        else if (l == 'е') priceLet = 1;
        else if (l == 'ё') priceLet = 3;
        else if (l == 'ж') priceLet = 4;
        else if (l == 'з') priceLet = 4;
        else if (l == 'и') priceLet = 1;
        else if (l == 'й') priceLet = 3;
        else if (l == 'к') priceLet = 2;
        else if (l == 'л') priceLet = 2;
        else if (l == 'м') priceLet = 2;
        else if (l == 'н') priceLet = 1;
        else if (l == 'о') priceLet = 1;
        else if (l == 'п') priceLet = 2;
        else if (l == 'р') priceLet = 2;
        else if (l == 'с') priceLet = 2;
        else if (l == 'т') priceLet = 2;
        else if (l == 'у') priceLet = 3;
        else if (l == 'ф') priceLet = 5;
        else if (l == 'х') priceLet = 4;
        else if (l == 'ц') priceLet = 5;
        else if (l == 'ч') priceLet = 5;
        else if (l == 'ш') priceLet = 4;
        else if (l == 'щ') priceLet = 5;
        else if (l == 'ъ') priceLet = 5;
        else if (l == 'ы') priceLet = 5;
        else if (l == 'ь') priceLet = 4;
        else if (l == 'э') priceLet = 5;
        else if (l == 'ю') priceLet = 5;
        else if (l == 'я') priceLet = 4;
        else priceLet = 0;
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
        else SoundManager.PlaySound("Response");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"
            && Vector2.Distance(transform.position, GameBase.G.player.destination) < 1f)
        {
            SoundManager.PlaySound("CollectLetter");
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
            spinnerRend.transform.localEulerAngles = new Vector3( 0, 0, -360 * rotationAnimationCurve.Evaluate( rotationSpeed * Time.time % 1 ) );
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
