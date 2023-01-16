using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    private Button b;
    
    private void OnEnable()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(() => AddToWord());
    }
    
    private void AddToWord()
    {
        Debug.Log(name);
    }
}
