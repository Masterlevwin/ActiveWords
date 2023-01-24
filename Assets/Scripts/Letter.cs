using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{
    public delegate void DelegateTake(Letter l);
    public DelegateTake takeNotify;
    
    public void OnPointerClick(PointerEventData eventData)
    {

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
