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
    public List<Vector2> spawns;
    
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
	if (spawns == null) spawns = new List<Vector2>();
        if (GameObject.Find("Word") == null)
	{
	    GameObject wordGO = new GameObject("Word");
	    wordAnchor = wordGO.transform;
	}
        char[] letters = new char[] { "Б", "А", "Б", "А"};
	for (int i = 0; i < letters.Length; i++) MakeLetter(letters[i]);
    }
    
    private void MakeLetter(char l)
    {
        GameObject letGO = Instantiate(prefabLetter);
	letGO.transform.SetParent(wordAnchor);
        letGO.transform.position = SpawnLetter();
	spawns.Add(letGO.transform.position);
        if (l == "А") letGO.GetComponent<SpriteRenderer>().sprite = letters[0];
	else if (l == "Б") letGO.GetComponent<SpriteRenderer>().sprite = letters[1];
        Letter let = LetGO.GetComponent<Letter>();
        word.Add(let);
    }
    
    private Vector2 SpawnLetter()
    {
    	Vector2 spawnLet = new Vector2(RandomWithoutFloat(-9f, 9f).x, RandomWithoutFloat(-4f, 4f).y);
	foreach (Vector2 v in spawns) if (v == spawnLet) return SpawnLetter();
	return spawnLet;
    }
    
    public static float RandomWithoutFloat(float from, float to, float without = 0f)
    {
        float res = Random.Range(from, to);
        if (res != without) return res;
        else return RandomWithoutFloat(from, to, without);
    }
}
