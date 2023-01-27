using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{ 
    public Vector2 posLet { private set; get; }
    
    private void OnEnable()
    {
        SetLetterPos(transform.position);
    }
    
    public void SetLetterPos(Vector2 pos)
    {
        posLet = pos;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        GameBase.G.RemoveAtWord(this);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameBase.G.AddToWord(this);
        }
    }
    
}
