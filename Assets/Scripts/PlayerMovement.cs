using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private Vector2 mousePos;
    private Rigidbody2D rb;
    private Canvas bar;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        bar = GetComponentInChildren<Canvas>();
    }

    void Update()
    {
        mousePos = cam.ScreenToWorldPoint( Input.mousePosition );
    }

    void FixedUpdate()
    {
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2( lookDir.y, lookDir.x ) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void LateUpdate()
    {
        bar.transform.LookAt( cam.transform.position );
        bar.transform.rotation = cam.transform.rotation;
    }
}
