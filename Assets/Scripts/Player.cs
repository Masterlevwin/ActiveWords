using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pathfinding;

public class Player : MonoBehaviour
{
    public float hitPlayer { private set; get; }
    public float maxHit;
    private TMP_Text txtHp;
    private Image bar;

    private Vector2 startPos;
    private Color colorPlayer;

    void Start()
    {
        colorPlayer = GetComponentInChildren<SpriteRenderer>().color;
        txtHp = GetComponentInChildren<TMP_Text>();
        bar = GetComponentsInChildren<Image>()[1];
        startPos = transform.position;
        hitPlayer = maxHit;
    }

    public void SetHit(float hit)
    {
        hitPlayer = hit;
        txtHp.text = $"{hitPlayer}";
        bar.fillAmount = hitPlayer / maxHit;
        if (hitPlayer <= 0)
        {
            GameBase.G.CompleteGame();
        } 
    }

    public void Damage(float dmg)
    {
        hitPlayer -= dmg;
        SetHit(hitPlayer);
    }
    
    public void SetPos(Vector2 pos)
    {
        if (pos == startPos && !gameObject.activeSelf) gameObject.SetActive(true);
        transform.position = pos;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color(colorPlayer.r, colorPlayer.g - .5f, colorPlayer.b - .5f, colorPlayer.a);
            Damage(1);
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
