using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour, IPointerClickHandler
{
    public float speed = 10f;
    
    void Start()
    {
        
    }

    void Update()
    {
        var mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Vector3.Lerp(transform.position, mp, Time.deltaTime * speed);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    
    }
}
