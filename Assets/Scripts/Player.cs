using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour
{
    private Vector2 startPos;
    public int hitPlayer = 3;
    private Color colorPlayer;

    void Start()
    {
        colorPlayer = GetComponentInChildren<SpriteRenderer>().color;
        startPos = GetPos();
    }

    public void SetPos(Vector2 pos)
    {
        if (pos == startPos && !gameObject.activeSelf) gameObject.SetActive(true);
        transform.position = pos;
        startPos = GetPos();
    }
    
    public Vector2 GetPos()
    {
        return transform.position;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && hitPlayer <= 0)
        {
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Player")
        {
            hitPlayer--;
            GetComponentInChildren<SpriteRenderer>().color = new Color(colorPlayer.r, colorPlayer.g - .5f, colorPlayer.b - .5f, colorPlayer.a);
        } 
        
        if (collision.gameObject.tag == "Teleport")
        {
            gameObject.SetActive(false);
            SetPos(startPos);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponentInChildren<SpriteRenderer>().color = colorPlayer;
        } 
    }
}
