using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    
    public Letter prefab;
    public Sprite[] letters;
    
    void Start()
    {
        if (G == null)
        {
            G = this;
        }
        else if (G == this)
        {
            Destroy(gameObject);
        }
        InitLevel();
    }

    public void InitLevel()
    {
        Letter let = Instantiate(prefab, new Vector2(Random.Range(-Screen.width/2,Screen.width/2), Random.Range(-Screen.height/2,Screen.height/2)), Quaternion.identity, transform.parent);
        let.GetComponent<SpriteRenderer>().sprite = letters[0];
    }
}
