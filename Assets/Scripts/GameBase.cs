using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    
    public TextAsset textAsset;
    
    public GameObject prefabLetter;
    public Sprite[] letters;
    public List<Letter> lets;
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
        if (lets == null) lets = new List<Letter>();
	if (spawns == null) spawns = new List<Vector2>();
        if (GameObject.Find("Word") == null)
	{
	GameObject wordGO = new GameObject("Word");
	wordAnchor = wordGO.transform;
	}
	
	string[] words = ParseText(textAsset.text);
	string wordLevel = word[0, word.Length];
	
	//string wordLevel = "АБАБ";
        char[] chars = wordLevel.ToCharArray();
	for (int i = 0; i < chars.Length; i++) MakeLetter(chars[i]);
    }
    
    private string[] ParseText(string st)
    {
    	string[] lines = st.Split("\n"[0]);
	return lines;
    }
    
    private void MakeLetter(char l)
    {
        GameObject letGO = Instantiate(prefabLetter);
	    letGO.transform.SetParent(wordAnchor);
        letGO.transform.position = SpawnLetter();
	    spawns.Add(letGO.transform.position);
        if (l == 'А') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[0];
	    else if (l == 'Б') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[1];
        Letter let = letGO.GetComponentInChildren<Letter>();
        lets.Add(let);
    }
    
    private Vector2 SpawnLetter()
    {
    	Vector2 spawnLet = new Vector2(RandomWithoutFloat(-8f, 8f), RandomWithoutFloat(-3f, 3f));
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
