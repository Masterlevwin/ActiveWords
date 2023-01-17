using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    
    public GameObject prefabLetter;
    public Sprite[] letters;
    public List<Letter> word;
    public Transform wordAnchor;
    
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
        if (GameObject.Find("Word") == null)
		{
			GameObject wordGO = new GameObject("Word");
			wordAnchor = wordGO.transform;
		}
        MakeLetters();
    }
    
    private void MakeLetters()
    {
        GameObject letGO = Instantiate(prefabLetter);
		letGO.transform.SetParent(wordAnchor);
        letGO.transform.position = new Vector2(RandomWithoutFloat(-9f, 9f).x, RandomWithoutFloat(-4f, 4f).y);
        letGO.GetComponent<SpriteRenderer>().sprite = letters[0];
		
        Letter let = LetGO.GetComponent<Letter>();
        word.Add(let);
    }
    
    public static float RandomWithoutFloat(float from, float to, float without = 0f)
    {
        float res = Random.Range(from, to);
        if (res != without) return res;
        else return RandomWithoutInt(from, to, without);
    }
}
