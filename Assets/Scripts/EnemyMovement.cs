using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Camera cam;
    private Vector2 mousePos;
    private Rigidbody2D rb;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        mousePos = GameBase.G.player.transform.position;
    }

    void FixedUpdate()
    {
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2( lookDir.y, lookDir.x ) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }
}