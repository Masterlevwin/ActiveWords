using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    
    public Letter prefab;
    public Wall wall;
    public Sprite[] letters;
    public List<Letter> word;
    
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
        if (word == null) word = new List<Letter>();
        
        Letter let = Instantiate(prefab, new Vector2(Random.Range(-Screen.width/2 + wall.transform.localScale.x, Screen.width/2 - wall.transform.localScale.x),
        Random.Range(-Screen.height/2 + wall.transform.localScale.y, Screen.height/2 - wall.transform.localScale.y)), Quaternion.identity, transform.parent);
        let.GetComponent<SpriteRenderer>().sprite = letters[0];
        
        word.Add(let);
    }
    
    private List<Letter> Word()
    {
        
    }
}
