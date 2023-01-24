using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{
    public delegate void Take(Letter l);
    public event Take takeNotify, cancelNotify;
    
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
        cancelNotify?.Invoke(this);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            takeNotify?.Invoke(this);
        }
    }
    
}
