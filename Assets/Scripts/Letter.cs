using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{ 
    public Vector3 posLet { private set; get; }
    public char charLet { private set; get; }

    private void OnEnable()
    {
        SetLetterPos(transform.position);
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
        if (GameBase.G.phase == GamePhase.game) GameBase.G.RemoveAtWord(this);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"
            && Vector2.Distance(transform.position, GameBase.G.player.destination) < 1f)
        {
            GameBase.G.AddToWord(this);
        }
    }
    
}