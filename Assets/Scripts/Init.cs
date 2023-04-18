using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Init : MonoBehaviour
{
    public TextAsset textAsset;
    private List<string> wordsFromTextAsset;
    private Dictionary<string, string> dictWords;
    public Button wordLevelButton;
    private TMP_Text wordLevelText;
    
    public GameObject prefabLetter, prefabCell, prefabLeaves, prefabPlate;
    private Transform wordAnchor, cellAnchor, blockAnchor;
    public GameObject[] animBlocks;
    public Sprite[] letters, cells;
    public LayerMask obtacleMask;
    public List<Letter> lets;
    public List<Vector2> letPositions; 
    private BoxCollider2D table;
    private Collider2D[] cols;

    void Start()
    {
        wordsFromTextAsset = ParseText(textAsset.text); // Загружаем в массив все слова из текстового ассета
        wordLevelText = wordLevelButton.GetComponentInChildren<TMP_Text>();

        if (GameObject.Find("Letters") == null)			// Создаем пустой объект в иерархии, чтобы спрятать туда сгенерированные буквы
	    {
	        GameObject wordGO = new GameObject("Letters");
	        wordAnchor = wordGO.transform;
	    }
	    if (GameObject.Find("Cells") == null)			// Создаем пустой объект в иерархии, чтобы спрятать туда конечные места для букв
        {
            GameObject cellGO = new GameObject("Cells");
            cellAnchor = cellGO.transform;
        }
	    if (GameObject.Find("Blocks") == null)			// Создаем пустой объект в иерархии, чтобы спрятать туда сгенерированные блоки
	    {
	        GameObject blockGO = new GameObject("Blocks");
	        blockAnchor = blockGO.transform;
	    }
	    table = GameObject.Find("Table").GetComponent<BoxCollider2D>();    // Сохраняем ссылку на коллайдер стола, к которому будем обращаться каждый раз, как потребуется спавнить объект
    }
    
    private List<string> ParseText(string txt)  // Метод преобразования текста в массив строк
    {
    	string[] lines = txt.Split( new char[] { ':', '\n' } );
	
	    if( dictWords == null ) dictWords = new Dictionary<string, string>();
	    for( int i = 0; i < lines.Length - 1; i += 2 )
	    {
	        dictWords.TryAdd( lines[i], lines[i + 1] );
	    }
	    List<string> words = new List<string>( dictWords.Keys );
	    return words;
    }

    private string InterpretationWord( string wordLevel )
    {
    	if( !dictWords.TryGetValue( wordLevel, out string str ) ) return wordLevel;
        return str;
    }
    
    public void ClearLetters()			// Метод очищения уровня
    {
        wordLevelText.text = $"";           	                                    // Очищаем отображение слова
        if (lets != null && lets.Count > 0) lets.Clear();				            // Очищаем список букв        
        if (letPositions != null && letPositions.Count > 0) letPositions.Clear();	// Очищаем список конечных мест
        foreach (Transform child in cellAnchor) Destroy(child.gameObject);          // Удаляем объекты конечных мест
        foreach (Transform child in wordAnchor) Destroy(child.gameObject);          // Удаляем объекты букв и платформ
        foreach (Transform child in blockAnchor) Destroy(child.gameObject);         // Удаляем объекты блоков}
    }

    public void Reset()				// Метод обновления уровня
    {
    	StopAllCoroutines();
        
        if (GameBase.G.gOs != null && GameBase.G.gOs.Count > 0)
        {
            foreach( GameObject g in GameBase.G.gOs ) { Destroy(g); }
            GameBase.G.gOs.Clear();
        }
        GameBase.G.StopAllCoroutines();

        ClearLetters();
        if ( GameBase.G._timer.gameObject.activeSelf ) GameBase.G._timer.StopTimer();
        if ( GameBase.G.player.gameObject.activeSelf ) GameBase.G.player.gameObject.SetActive(false);
        if ( GameBase.G.enemy.gameObject.activeSelf ) GameBase.G.enemy.gameObject.SetActive(false);
        if ( GameBase.G.levelUP.gameObject.activeSelf ) GameBase.G.levelUP.gameObject.SetActive(false);
        if ( GameBase.G.gameOver.gameObject.activeSelf ) GameBase.G.gameOver.gameObject.SetActive(false);
	    if ( GameBase.G.continueArea.gameObject.activeSelf ) GameBase.G.continueArea.gameObject.SetActive(false);
	    InitLevel();				// Инициализируем новый уровень
    }
    
    private void InitLevel()			// Метод инициализации уровня
    {
        GameBase.G.phase = GamePhase.init;      // Переводим игру в фазу инициализации уровня, запрещая двигать персонажа
	    CreateBlocks();			    // Создаем блоки деревьев и т.п.
        CreateLetters();			// Создаем буквы уровня
        GameBase.G.TrainingView();  // Запускаем советы игры
    }

    public void SetupLevel( string wordLevel )
    {
        if ( GameBase.level <= 2 ) wordLevelText.text = wordLevel;
        else if ( GameBase.level == 3 ) wordLevelText.text = InterpretationWord( wordLevel );
        else if ( GameBase.level > 3 && Random.Range(0, 3) == 0 ) wordLevelText.text = InterpretationWord( wordLevel );
        else wordLevelText.text = wordLevel;
    }
    
    private void CreateBlocks()			// Метод создания блоков препятствий
    {
    	int numBlocks = Random.Range(1, 10);	// Выбираем случайное количество блоков
	    for (int i = 0; i < numBlocks; i++)
	    {
            Instantiate( animBlocks[Random.Range( 0, animBlocks.Length )], Spawn(), Quaternion.identity, blockAnchor );
        } 
    }
    
    private void CreateLeaves()					    // Метод создания бонуса листиков
    {
    	Instantiate( prefabLeaves, Spawn(), Quaternion.identity, blockAnchor );
    }

    public Vector2 Spawn()						    // Метод генерации случайной точки спавна
    {
    	float x, y;								    // Выбираем случайные значения в пределах стола, основываясь на костях его коллайдера
	    x = Random.Range(table.transform.position.x - Random.Range(0, table.bounds.extents.x), table.transform.position.x + Random.Range(0, table.bounds.extents.x));
	    y = Random.Range(table.transform.position.y - Random.Range(0, table.bounds.extents.y), table.transform.position.y + Random.Range(0, table.bounds.extents.y));
	    Vector2 spawnPoint = new Vector2(x, y);		// Создаем точку спавна
	    if (Point(spawnPoint)) return spawnPoint;	// Если точка доступна, возвращаем её из метода,
	    else return Spawn();						// иначе ищем снова доступную точку
	
	    bool Point(Vector2 spawn)					// Внутренний метод проверки доступности точки
	    {
	        Vector2 size = new Vector2(2f, 2f);		// Дистанция коллайдеров между объектами
	        cols = Physics2D.OverlapBoxAll(spawn, size, 0f, obtacleMask);	// Определяем массив коллайдеров на столе,пересекающий точку спавна в данный момент
	        if (cols.Length > 0) return false;		// Если такие коллайдеры есть, точка спавна не доступна,
	        else return true;						// иначе доступна
	    }
    }
    
    private void CreateCells()						// Метод создания конечных мест букв
    {
    	if (letPositions == null) letPositions = new List<Vector2>();   // Создаем список конечных мест букв
    	GameObject c;								// Объявляем переменную GameObject
        Vector2 cellPos;							// Объявляем переменную Vector2
        for (int i = 0; i < lets.Count; i++) 
        {	    // Взависимости от количества букв сгенерированного слова уровня определяем позицию каждого места
            float cen = (float)lets.Count / 2.5f;
            cellPos = new Vector2( .8f - cen + i * .8f, -4f );
            c = Instantiate(prefabCell, cellPos, Quaternion.identity, cellAnchor);	// Создаем конечное место буквы
            c.GetComponent<SpriteRenderer>().sprite = cells[0];				        // Определяем спрайт этого места
            letPositions.Add(cellPos);							                    // Добавляем это место в список позиций
        }
		        // Далее создаем крайние блоки для оформления интерактивной зоны
        c = Instantiate(prefabCell, new Vector2(letPositions[0].x - .4f, letPositions[0].y), Quaternion.identity, cellAnchor);
        c.GetComponent<SpriteRenderer>().sprite = cells[1];
        c = Instantiate(prefabCell, new Vector2(letPositions[letPositions.Count - 1].x + .4f, letPositions[letPositions.Count - 1].y), Quaternion.identity, cellAnchor);
        c.GetComponent<SpriteRenderer>().sprite = cells[2];
    }
        
    private Sprite SetLetterSprite(char l)		    // Метод установки спрайта на объект буквы
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

    private void CreateLetters()							// Метод создания букв на столе
    {
    	if (lets == null) lets = new List<Letter>();		// Создаем список букв
    	string wordLevel = wordsFromTextAsset[Random.Range(0, wordsFromTextAsset.Count)];	// Выбираем слово для уровня из списка
    	SetupLevel( wordLevel );				            // Определяем сложность уровня
    	char[] chars = wordLevel.ToCharArray();			    // Преобразуем выбранное слово в массив символов (букв)
    	StartCoroutine(MakeLetter(chars));	                // Рисуем каждую букву через каждые полсекунды
    }

    private IEnumerator MakeLetter(char[] chars)	        // Метод создания каждой буквы
    {
        for ( int i = 0; i < chars.Length; i++ )
        {
            GameObject letGO = Instantiate(prefabLetter, Spawn(), Quaternion.identity, wordAnchor); // Инициализируем объект буквы
	        letGO.GetComponent<SpriteRenderer>().sprite = SetLetterSprite(chars[i]);                // Устанавливаем спрайт буквы
	        Letter let = letGO.GetComponent<Letter>();		// Получаем компонент Letter созданной буквы
            let.SetLetterPos(let.transform.position);		// Запоминаем начальную позицию буквы
            let.SetChar(chars[i]);							// Устанавливаем символ для дальнейшей проверки этого свойства
	        lets.Add(let);									// Добавляем букву в список
		
	        if( GameBase.level > 3 && Random.Range(0,4) == 0 ) {
                Instantiate( prefabPlate, let.transform.position, Quaternion.identity, wordAnchor );    // Инициализируем объект платформы
            }
            yield return new WaitForSeconds(.6f);           // Делаем паузу
        }                                         
        CreateCells();                                      // Создаем конечные места букв
	    if( GameBase.level > 4 ) CreateLeaves();            // Создаем бонус листиков в случайном доступном месте
        GameBase.G.StartGame();                             // Запускаем игру
    }

    private void Update()
    {
        if( GameBase.G.phase != GamePhase.game ) wordLevelButton.interactable = false;
        else wordLevelButton.interactable = true;
    }
}
