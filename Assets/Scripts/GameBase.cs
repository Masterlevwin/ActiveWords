using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    public static GameBase G;
    
    public TextAsset textAsset;
    private string[] wordsFromText;
    private Transform wordAnchor;
    public Text wordLevelText;
    
    public GameObject prefabLetter;
    public Sprite[] letters;
    private List<Letter> lets;
    private List<Vector2> spawns;
    
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
	
	wordsFromText = ParseText(textAsset.text);	// Загружаем в массив все слова из текстового ассета
	if (GameObject.Find("Word") == null)		// Создаем пустой объект в иерархии, чтобы спрятать туда сгенерированные буквы
	{
	    GameObject wordGO = new GameObject("Word");
	    wordAnchor = wordGO.transform;
	}
        InitLevel(wordsFromText);
    }
    
    private string[] ParseText(string txt)
    {
    	string[] lines = txt.Split("\n");
	return lines;
    }
    
    private void InitLevel(string[] words)
    {
	string wordLevel = words[Random.Range(0, words.Length)];	// Выбираем слово для уровня из массива
	wordLevelText.text = wordLevel;					// Отображаем это слово в канвасе - временно для отладки
        char[] chars = wordLevel.ToCharArray();				// Преобразуем выбранное слово в массив символов (букв)
	for (int i = 0; i < chars.Length; i++) MakeLetter(chars[i]);	// Рисуем каждую букву
    }
    
    private void MakeLetter(char l)
    {
        GameObject letGO = Instantiate(prefabLetter);		// Инициализируем объект буквы
	letGO.transform.SetParent(wordAnchor);			// Прячем её в иерархии
	
	if (spawns == null) spawns = new List<Vector2>();	// Создаем список мест для букв
	letGO.transform.position = SpawnLetter();		// Определяем место буквы на игровом поле
	spawns.Add(letGO.transform.position);			// Полученное место добавляем в список занятых
	
        letGO.GetComponentInChildren<SpriteRenderer>().sprite = SetLetterSprite(l);	// Устанавливаем спрайт буквы
	if (lets == null) lets = new List<Letter>();			// Создаем список букв
        Letter let = letGO.GetComponentInChildren<Letter>();		// Сохраняем ссылку на класс буквы дочернего объекта
        lets.Add(let);							// Добавляем букву в список для дальнейшей работы со списком
    }
    
    private Vector2 SpawnLetter()
    {
     	Vector2 spawnLet = new Vector2(Random.Range(-8f, 8f), Random.Range(-3f, 3f));
	
	foreach (Vector2 v in spawns) if (v == spawnLet) return SpawnLetter();
	return spawnLet;
    }
    
    private Sprite SetLetterSprite(char l)
    {
    	Sprite spLet = null;
    	if (l == 'а') spLet = letters[0];
        else if (l == 'б') spLet = letters[1];
        else if (l == 'в') spLet = letters[2];
        else if (l == 'г') spLet = letters[3];
        else if (l == 'д') spLet = letters[4];
        else if (l == 'е') spLet = letters[5];
        else if (l == 'ё') spLet = letters[6];
        else if (l == 'ж') spLet = letters[7];
        else if (l == 'з') spLet = letters[8];
        else if (l == 'и') spLet = letters[9];
        else if (l == 'й') spLet = letters[10];
        else if (l == 'к') spLet = letters[11];
        else if (l == 'л') spLet = letters[12];
        else if (l == 'м') spLet = letters[13];
        else if (l == 'н') spLet = letters[14];
        else if (l == 'о') spLet = letters[15];
        else if (l == 'п') spLet = letters[16];
        else if (l == 'р') spLet = letters[17];
        else if (l == 'с') spLet = letters[18];
        else if (l == 'т') spLet = letters[19];
        else if (l == 'у') spLet = letters[20];
        else if (l == 'ф') spLet = letters[21];
        else if (l == 'х') spLet = letters[22];
        else if (l == 'ц') spLet = letters[23];
        else if (l == 'ч') spLet = letters[24];
        else if (l == 'ш') spLet = letters[25];
        else if (l == 'щ') spLet = letters[26];
        else if (l == 'ъ') spLet = letters[27];
        else if (l == 'ы') spLet = letters[28];
        else if (l == 'ь') spLet = letters[29];
        else if (l == 'э') spLet = letters[30];
        else if (l == 'ю') spLet = letters[31];
        else if (l == 'я') spLet = letters[32];
        return spLet;
    }
	
    public static float RandomWithoutFloat(float from, float to, float without = 0f)
    {
        float res = Random.Range(from, to);
        if (res != without) return res;
        else return RandomWithoutFloat(from, to, without);
    }
}
