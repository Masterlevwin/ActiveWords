using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Letter : MonoBehaviour, IPointerClickHandler
{ 
    public Vector3 posLet { private set; get; }

    private void OnEnable()
    {
        SetLetterPos(transform.position);

    }
    
    public void SetLetterPos(Vector3 pos)
    {
        posLet = pos;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(name);
        GameBase.G.RemoveAtWord(this);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Vector2.Distance(transform.position, GameBase.G.player.destination) < 1f)
        {
            //Debug.Log(name);
            GameBase.G.AddToWord(this);
        }
    }
    
}
