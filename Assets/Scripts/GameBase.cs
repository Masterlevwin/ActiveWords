using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    
    public GameObject prefab;
    public Wall wall;
    public Sprite[] letters;
    public List<GameObject> word;
    
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
        if (word == null) word = new List<GameObject>();
        GameObject ground = GameObject.Find("Ground");

        GameObject let = Instantiate(prefab, new Vector2(Random.Range(-10, 10), Random.Range(-5, 5)), Quaternion.identity, ground.transform);
        //let.GetComponent<SpriteRenderer>().sprite = letters[0];

        
        word.Add(let);
    }
    
    private List<GameObject> Word()
    {
        return word;
    }
}
