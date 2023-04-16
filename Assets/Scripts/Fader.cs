using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour
{ 
    bool fadeIn; 
    bool fadeOut; 
    float alphaColor;

    public Image fadeImage;     //Черная картинка растянутая на весь экран, с 0 прозрачностью и выключенным рейкаст таргетом, ее можно будет отключить в инспекторе, чтобы не мешала
    public int loadScene = 0;   //Номер загружаемой сцены
    public Color fadeInColor;   //Выбираем желаемый цвет при окончании сцены
    public Color fadeOutColor;  //Выбираем желаемый цвет при старте сцены

    private void Start()        //Вызывается автоматически при старте сцены
    { 
        fadeImage.gameObject.SetActive(true);                                               //Включаем картинку
        fadeImage.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, 1f);    //Ставим стартовый цвет
        fadeOut = true;                                                                     //Запускаем процесс 
    }

    public void ButtonStartGame()           //Вызывается из UI
    { 
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(fadeInColor.r, fadeInColor.g, fadeInColor.b, 0);
        fadeIn = true;
    }

    private void StartScene()               //Метод запускающий сцену, вызывается в апдейте, когда прозрачность картинки станет достаточно низкой, т.е. экран затемнится
    { 
        SceneManager.LoadScene(loadScene);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("ВЫХОД");
    }

    private void Update()
    {
        if (fadeIn)
        {
            alphaColor = Mathf.Lerp(fadeImage.color.a, 1, Time.deltaTime * 2.5f);
            fadeImage.color = new Color(fadeInColor.r, fadeInColor.g, fadeInColor.b, alphaColor);
        }

        if (fadeOut)
        {
            alphaColor = Mathf.Lerp(fadeImage.color.a, 0, Time.deltaTime * 2.5f);
            fadeImage.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, alphaColor);
        }

        if (alphaColor > 0.95 && fadeIn)
        {
            fadeIn = false;
            StartScene();                           // Вызываем метод со стартом нужной сцены
        }

        if (alphaColor < 0.05 && fadeOut)
        {
            fadeOut = false;
            fadeImage.gameObject.SetActive(false);  // Отключаем картинку, чтобы не было прозрачной картинки на весь экран во время игры, иначе может негативно сказаться на производительности
        }
    }

    public void FaderLevel()    // Вспомогательный метод для загрузки уровня без перезапуска сцены
    {
        fadeImage.gameObject.SetActive(true);                                               //Включаем картинку
        fadeImage.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, 1f);    //Ставим стартовый цвет
        fadeOut = true;                                                                     //Запускаем прозрачность
    }
}
