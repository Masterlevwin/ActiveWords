using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour
{
    public int hitPlayer = 3;
    private Color colorPlayer;

    void Start()
    {
        colorPlayer = GetComponentInChildren<SpriteRenderer>().color;
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
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponentInChildren<SpriteRenderer>().color = colorPlayer;
        } 
    }
}
