using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Threading;

public class Init : MonoBehaviour
{
    static public CancellationTokenSource cancelTokenSource;
    static public CancellationToken token;
    
    public TextAsset textAsset;
    private string[] wordsFromTextAsset;
    public TMP_Text wordLevelText;
    
    private Transform wordAnchor;
    public GameObject prefabLetter;
    public Sprite[] letters;
    public List<Letter> lets; 
    public LayerMask obtacleMask;
    
    private Transform blockAnchor;
    public GameObject prefabBlock;
    public Sprite[] blocks;
    public GameObject prefabTeleport;
    
    private Transform cellAnchor;
    public GameObject prefabCell;
    public Sprite[] cells;
    public List<Vector2> letPositions;
    
    private BoxCollider2D table;
    private Collider2D[] cols;

    void Start()
    {
    	if (cancelTokenSource == null)				// Создаем токен для отмены ассихронных задач
	{
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
	}
        wordsFromTextAsset = ParseText(textAsset.text);		// Загружаем в массив все слова из текстового ассета
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
	    GameObject blockGO = new GameObject("Blocs");
	    blockAnchor = blockGO.transform;
	}
	table = GameObject.Find("BG").GetComponent<BoxCollider2D>();	// Сохраняем ссылку на коллайдер стола,
									// к которому будем обращаться каждый раз, как потребуется спавнить объект
	InitLevel();				// Инициализируем уровень
    }
    
    private string[] ParseText(string txt)	// Метод преобразования текста в массив строк
    {
    	string[] lines = txt.Split("\n");
	    return lines;
    }
    
    public void Reset()				// Метод обновления уровня, временно вызывается кнопкой
    {
    	cancelTokenSource.Dispose();                        	// Освобождаем ресурсы
        cancelTokenSource = new CancellationTokenSource();  	// Создаем новый токен для ассинхронных задач
        token = cancelTokenSource.Token;                    	// Обновляем токен
    	
        GameBase.G.player.SetPath(null);			// Останавливаем поиск пути у игрока
        GameBase.G.enemy.SetPath(null);				// Останавливаем поиск пути у бота
	wordLevelText.text = $"";				// Очищаем отображение слова
    	if (lets != null && lets.Count > 0) lets.Clear();	// Очищаем список букв        
        if (letPositions != null && letPositions.Count > 0) letPositions.Clear();	// Очищаем список конечных мест
	foreach (Transform child in cellAnchor) Destroy(child.gameObject);		// Удаляем объекты конечных мест
	foreach (Transform child in wordAnchor) Destroy(child.gameObject);		// Удаляем объекты букв
	foreach (Transform child in blockAnchor) Destroy(child.gameObject);		// Удаляем объекты блоков
	InitLevel();						// Инициализируем новый уровень
    }
    
    private async void InitLevel()					// Метод инициализации уровня
    {
    	GameBase.G.phase = GamePhase.init;			// Переводим игру в фазу инициализации уровня, запрещая двигать персонажа
	CreateBlocks();							// Создаем блоки препятствий
	await CreateLetters();						// Создаем буквы уровня
	CreateCells();                                          // Создаем конечные места букв
    GameBase.G.player.GetComponent<Player>().SetPos(Spawn());	// Устанавливаем позицию игрока
	GameBase.G.enemy.transform.position = Spawn();			// Устанавливаем позицию бота
        GameBase.G.StartGame();					// Запускаем игру
    }

    private void CreateBlocks()// Метод создания блоков препятствий
    {
    	int numBlocks = Random.Range(1, 10);// Выбираем случайное количество блоков
	for (int i = 0; i < numBlocks; i++)
	{
	    GameObject block = Instantiate(prefabBlock, Spawn(), Quaternion.identity, blockAnchor);	// Создаем блок в доступном месте
	    block.GetComponent<SpriteRenderer>().sprite = blocks[Random.Range(0, blocks.Length)];	// Устанавливаем случайный спрайт блока
	}
		// Со второго уровня создаем блок-телепорт
	if (GameBase.level > 2)
    {
        GameObject teleport = Instantiate(prefabTeleport, Spawn(), Quaternion.identity, blockAnchor);
    } 
    }
    
    private Vector2 Spawn()
    {
    	float x, y;// Выбираем случайные значения в пределах стола, основываясь на костях его коллайдера
	x = Random.Range(table.transform.position.x - Random.Range(0, table.bounds.extents.x), table.transform.position.x + Random.Range(0, table.bounds.extents.x));
	y = Random.Range(table.transform.position.y - Random.Range(0, table.bounds.extents.y), table.transform.position.y + Random.Range(0, table.bounds.extents.y));
	Vector2 spawnPoint = new Vector2(x, y);					// Создаем точку спавна
	if (Point(spawnPoint)) return spawnPoint;				// Если точка доступна, возвращаем её из метода,
	else return Spawn();							// иначе ищем снова доступную точку
	
	bool Point(Vector2 spawn)						// Внутренний метод проверки доступности точки
	{
	    Vector2 size = new Vector2(2f, 2f);					// Дистанция коллайдеров между объектами
	    cols = Physics2D.OverlapBoxAll(spawn, size, 0f, obtacleMask);	// Определяем массив коллайдеров на столе,пересекающий точку спавна в данный момент
	    if (cols.Length > 0) return false;					// Если такие коллайдеры есть, точка спавна не доступна,
	    else return true;							// иначе доступна
	}
    }
    
    private void CreateCells()
    {
    	if (letPositions == null) letPositions = new List<Vector2>();		// Создаем список конечных мест букв
    	GameObject c;									// Объявляем переменную GameObject
        Vector2 cellPos;								// Объявляем переменную Vector2
        for (int i = 0; i < lets.Count; i++) 
        {	// Взависимости от количества букв сгенерированного слова уровня определяем позицию каждого места
            cellPos = new Vector2(.5f - lets.Count/2 + i, -4.5f);
            c = Instantiate(prefabCell, cellPos, Quaternion.identity, cellAnchor);	// Создаем конечное место буквы
            c.GetComponent<SpriteRenderer>().sprite = cells[0];				// Определяем спрайт этого места
            letPositions.Add(cellPos);							// Добавляем это место в список позиций
        }
		// Далее создаем крайние блоки для оформления интерактивной зоны
        c = Instantiate(prefabCell, new Vector2(letPositions[0].x - 2f, letPositions[0].y), Quaternion.identity, cellAnchor);
        c.GetComponent<SpriteRenderer>().sprite = cells[1];
        c = Instantiate(prefabCell, new Vector2(letPositions[letPositions.Count - 1].x + 2f, letPositions[letPositions.Count - 1].y), Quaternion.identity, cellAnchor);
        c.GetComponent<SpriteRenderer>().sprite = cells[2];
    }
    
    private async Task CreateLetters()							// Метод создания букв на столе
    {
    	if (lets == null) lets = new List<Letter>();					// Создаем список букв
    	string wordLevel = wordsFromTextAsset[Random.Range(0, wordsFromTextAsset.Length)];		// Выбираем слово для уровня из массива
    	wordLevelText.text = wordLevel;							// Отображаем это слово в канвасе - временно для отладки
    	char[] chars = wordLevel.ToCharArray();						// Преобразуем выбранное слово в массив символов (букв)
    	for (int i = 1; i < chars.Length-1; i++) await MakeLetter(chars[i], token);	// Рисуем каждую букву
    }
    
    private async Task MakeLetter(char l, CancellationToken token = default)	        // Метод создания каждой буквы
    {                                              
        if (token.IsCancellationRequested) return;	// Проверяем наличие сигнала отмены задачи и выходим из метода и тем самым завершаем задачу
        await Task.Delay(500);                          // Ожидаем полсекунды
	
        GameObject letGO = Instantiate(prefabLetter, Spawn(), Quaternion.identity, wordAnchor);		// Инициализируем объект буквы
	letGO.GetComponent<SpriteRenderer>().sprite = SetLetterSprite(l);               // Устанавливаем спрайт буквы
	Letter let = letGO.GetComponent<Letter>();					// Получаем компонент Letter созданной буквы
        let.SetLetterPos(let.transform.position);					// Запоминаем начальную позицию буквы
        let.SetChar(l);									// Устанавливаем символ для дальнейшей проверки этого свойства
	lets.Add(let);									// Добавляем букву в список
    }
        
    private Sprite SetLetterSprite(char l)		// Метод установки спрайта на объект буквы
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
