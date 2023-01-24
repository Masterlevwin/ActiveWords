using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{
    public Vector2 posLet { get; private set };
    
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
        GameBase.G.cancelNotify(this);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameBase.G.takeNotify(this);
        }
    }
    
}
