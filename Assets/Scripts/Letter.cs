using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour
{
    public delegate void DelegateTake(Letter l);
    public DelegateTake takeNotify;
    
    public Vector2 posLet { get; private set };
    
    private void OnEnable()
    {
        SetLetterPos(transform.position);
    }
    
    public void SetLetterPos(Vector2 pos)
    {
        posLet = pos;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            takeNotify(this);
            Destroy(gameObject);
        }
    }
}
