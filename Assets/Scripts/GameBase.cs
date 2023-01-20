using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    
    public TextAsset textAsset;
    public Text wordLevelText;
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
	string wordLevel = words[Random.Range(0, words.Length)];
	wordLevelText.text = wordLevel;
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
        if (l == 'а') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[0];
	    else if (l == 'б') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[1];
        else if (l == 'в') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[2];
        else if (l == 'г') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[3];
        else if (l == 'д') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[4];
        else if (l == 'е') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[5];
        else if (l == 'ё') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[6];
        else if (l == 'ж') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[7];
        else if (l == 'з') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[8];
        else if (l == 'и') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[9];
        else if (l == 'й') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[10];
        else if (l == 'к') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[11];
        else if (l == 'л') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[12];
        else if (l == 'м') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[13];
        else if (l == 'н') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[14];
        else if (l == 'о') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[15];
        else if (l == 'п') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[16];
        else if (l == 'р') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[17];
        else if (l == 'с') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[18];
        else if (l == 'т') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[19];
        else if (l == 'у') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[20];
        else if (l == 'ф') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[21];
        else if (l == 'х') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[22];
        else if (l == 'ц') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[23];
        else if (l == 'ч') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[24];
        else if (l == 'ш') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[25];
        else if (l == 'щ') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[26];
        else if (l == 'ъ') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[27];
        else if (l == 'ы') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[28];
        else if (l == 'ь') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[29];
        else if (l == 'э') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[30];
        else if (l == 'ю') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[31];
        else if (l == 'я') letGO.GetComponentInChildren<SpriteRenderer>().sprite = letters[32];
        Letter let = letGO.GetComponentInChildren<Letter>();
        lets.Add(let);
    }
    
    private Vector2 SpawnLetter()
    {
    	Vector2 radiusLet = new Vector2(0.1f, 0.1f);
    	Vector2 spawnLet = new Vector2(RandomWithoutFloat(-8f, 8f), RandomWithoutFloat(-3f, 3f));
	    foreach (Vector2 v in spawns) if (v + radiusLet == spawnLet) return SpawnLetter();
	    return spawnLet;
    }
    
    public static float RandomWithoutFloat(float from, float to, float without = 0f)
    {
        float res = Random.Range(from, to);
        if (res != without) return res;
        else return RandomWithoutFloat(from, to, without);
    }
}
