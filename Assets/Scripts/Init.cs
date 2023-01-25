using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Threading;

public class Init : MonoBehaviour
{    
    public TextAsset textAsset;
    private string[] wordsFromTextAsset;
    
    public TMP_Text wordLevelText;
    private Transform wordAnchor;
    
    public GameObject prefabLetter;
    public LayerMask obtacleMask;
    
    public Sprite[] letters;
    public List<Letter> lets;
    
    private BoxCollider2D table;
    private Vector2 pos;
    private Collider2D[] cols;
    
    static public CancellationTokenSource cancelTokenSource;
    static public CancellationToken token;
    
    void Start()
    {
    	if (cancelTokenSource == null)
	{
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
	}
	
        wordsFromTextAsset = ParseText(textAsset.text);		// Загружаем в массив все слова из текстового ассета
	if (GameObject.Find("Words") == null)			// Создаем пустой объект в иерархии, чтобы спрятать туда сгенерированные буквы
	{
	    GameObject wordGO = new GameObject("Words");
	    wordAnchor = wordGO.transform;
	}
	table = GameObject.Find("BG").GetComponent<BoxCollider2D>();
	pos = table.transform.position;
	InitLevel(wordsFromTextAsset);
    }
    
    private string[] ParseText(string txt)
    {
    	string[] lines = txt.Split("\n");
	    return lines;
    }
    
    public void Reset()
    {
    	cancelTokenSource.Dispose();                        // Освобождаем ресурсы
        cancelTokenSource = new CancellationTokenSource();  // Создаем новый токен для ассинхронных задач
        token = cancelTokenSource.Token;                    // Обновляем токен
    	
	wordLevelText.text = $"";
    	if (lets != null && lets.Count > 0) lets.Clear();
	foreach (Transform child in wordAnchor) Destroy(child.gameObject);
	InitLevel(wordsFromTextAsset);
    }
    
    private async void InitLevel(string[] words)
    {
    	GameBase.G.phase = GamePhase.pause;
    	if (lets == null) lets = new List<Letter>();
    	string wordLevel = words[Random.Range(0, words.Length)];			// Выбираем слово для уровня из массива
    	wordLevelText.text = wordLevel;							// Отображаем это слово в канвасе - временно для отладки
    	char[] chars = wordLevel.ToCharArray();						// Преобразуем выбранное слово в массив символов (букв)
    	for (int i = 0; i < chars.Length; i++) await MakeLetter(chars[i], token);	// Рисуем каждую букву
	GameBase.G.StartGame();
    }
    
    private async Task MakeLetter(char l, float delay = 1f, CancellationToken token = default)	// Рисуем каждую букву с интервалом в секунду по умолчанию
    {                                              
        if (token.IsCancellationRequested) return;	// Проверяем наличие сигнала отмены задачи и выходим из метода и тем самым завершаем задачу
        await Task.Delay(500);
	
        GameObject letGO = Instantiate(prefabLetter);			    		// Инициализируем объект буквы
	letGO.transform.SetParent(wordAnchor);				        	// Прячем её в иерархии
        letGO.transform.position = Spawn();				            	// Определяем позицию буквы на сцене
	letGO.GetComponentInChildren<SpriteRenderer>().sprite = SetLetterSprite(l);     // Устанавливаем спрайт буквы
	Letter let = letGO.GetComponent<Letter>();
        let.SetLetterPos(let.transform.position);
	lets.Add(let);
    }
    
    private Vector2 Spawn()
    {
    	float x, y;
	    x = Random.Range(pos.x - Random.Range(0, table.bounds.extents.x), pos.x + Random.Range(0, table.bounds.extents.x));
	    y = Random.Range(pos.y - Random.Range(0, table.bounds.extents.y), pos.y + Random.Range(0, table.bounds.extents.y));
	    Vector2 spawnLet = new Vector2(x, y);

	    if (Point(spawnLet)) return spawnLet;
	    else return Spawn();
	    bool Point(Vector2 spawn)
	    {
	        Vector2 size = new Vector2(2f, 2f);
	        cols = Physics2D.OverlapBoxAll(spawn, size, 0f, obtacleMask);
            	if (cols.Length > 0) return false;
	        else return true;
	    }
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
}
