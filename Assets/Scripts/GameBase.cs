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

        GameObject let = Instantiate(prefab, new Vector2(Random.Range(-9, 9), Random.Range(-4, 4)), Quaternion.identity, ground.transform);
        //let.GetComponent<SpriteRenderer>().sprite = letters[0];

        
        word.Add(let);
    }
    
    private List<GameObject> Word()
    {
        return word;
    }
    
    public static float RandomWithoutInt(float from, float to, float without = 0f)
    {
        float res = Random.Range(from, to);
        if (res != without) return res;
        else return RandomWithoutInt(from, to, without);
    }
}
