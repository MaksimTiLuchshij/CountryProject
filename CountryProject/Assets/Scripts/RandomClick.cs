using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public class RandomClick : MonoBehaviour
{   //Массивы со спрайтами стран по материкам
    private Sprite[] sprites;
    private string country;
    public Image image;
    public Text[] texts;
    public Text textPoints;
    public GameObject answerButtons;
    public GameObject textInputPanel;
    public InputField inputField;
    public GameObject losePanel;
    public GameObject recordPanel;
    public InputField inputRecord;
    public GameObject[] buttons;
    public Canvas mainCanvas;
    int answers;
    int points = 0;
    int loseCount = 0;
    int randNum;
    string queryProp;
    static string mode;
    private Button activeButton;
    private Color activeColor;
    System.Random random = new System.Random();
    private RectTransform rectTransform;
    public Text namePlayer;


    public GameObject exitConfirm;
    void Start()
    {
        queryProp = ScrollModeScript.mode;
        switch (ScrollModeScript.mode)
        {
            case "Сложный":
                textInputPanel.SetActive(true);
                activeButton = textInputPanel.GetComponentInChildren<Button>();
                break;
            case "Викторина":
                answerButtons.SetActive(true);
                answers = 4;
                break;
            case "Легкий":
                answerButtons.SetActive(true);
                answers = 2;
                buttons[2].SetActive(false);
                buttons[3].SetActive(false);
                break;
        }
        switch (ScrollModeScript.mainland)
        {
            case "Весь мир":
                sprites = Resources.LoadAll<Sprite>("Countries/all");
                break;
            case "Европа":

                sprites = Resources.LoadAll<Sprite>("Countries/eu");
                break;
            case "Азия":
                sprites = Resources.LoadAll<Sprite>("Countries/as"); ;
                break;
            case "Северная Америка":
                sprites = Resources.LoadAll<Sprite>("Countries/na"); ;
                break;
            case "Южная Америка":
                sprites = Resources.LoadAll<Sprite>("Countries/sa"); ;
                break;
            case "Австралия и Океания":
                sprites = Resources.LoadAll<Sprite>("Countries/au"); ;
                break;
            case "Африка":
                sprites = Resources.LoadAll<Sprite>("Countries/eu"); ;
                break;
        }
        //Перемешивает числа и берет рандомные варианты ответов
        var nums = Enumerable.Range(0, sprites.Length).OrderBy(n => random.Next()).ToArray();
        for (int i = 0; i < answers; i++)
        {
            texts[i].text = sprites[nums[i]].name;
        }

        //выдает флаг, чтобы не было пустого Image
        randNum = random.Next(0, answers);
        image.GetComponent<Image>().sprite = sprites[nums[randNum]];
        country = sprites[nums[randNum]].name;
        //изменяет размер флагов, чтобы они не теряли пропорции
        float width = sprites[nums[randNum]].rect.width;
        float height = sprites[nums[randNum]].rect.height;
        rectTransform = image.GetComponent<RectTransform>();
        rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 31.05f, 330);
        rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (1280 - (width / height) * 330) / 2, (width / height) * 330);
    }

    public void TextAnswer()
    {
        if (inputField.text.ToLower().Equals(country.ToLower()))
        {
            points++;
            activeColor = Color.green;
        }
        else
        {
            points -= 2;
            activeColor = Color.red;
            loseCount++;
            if (loseCount == 5)
            {
                losePanel.SetActive(true);
                CheckRecord();
            }
        }
        textPoints.text = "Очки: " + points;
        inputField.text = "";
        StartCoroutine(WaitSecond());
    }
    //Считает очки взависимости от ответа
    public void AssighPoint(Button button)
    {
        activeButton = button;
        if (Convert.ToInt32(button.tag).Equals(randNum + 1))
        {
            points++;
            activeColor = Color.green;
        }
        else
        {
            points -= 2;
            loseCount++;
            activeColor = Color.red;
            if (loseCount == 5)
            {
                losePanel.SetActive(true);
                CheckRecord();
            }
        }
        textPoints.text = "Очки: " + points;
        StartCoroutine(WaitSecond());
    }

    private IEnumerator WaitSecond()
    {
        ColorBlock defaultColor = activeButton.colors;
        SetActiveButtons(false);
        SetButtonColor(activeButton, activeColor);
        yield return new WaitForSeconds(1);
        SetActiveButtons(true);
        activeButton.colors = defaultColor;
        Start();
    }
    private void SetActiveButtons(bool active)
    {
        if (activeButton.name == "Enter")
        {
            activeButton.enabled = active;
        }
        else
            for (int i = 0; i < answers; i++)
            {
                answerButtons.GetComponentsInChildren<Button>()[i].enabled = active;
            }
    }

    private void SetButtonColor(Button button, Color newColor)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = newColor;
        button.colors = colorBlock;
    }
    public void closeRandomClick()
    {
        exitConfirm.SetActive(true);
    }

    public void ExitToMenu()
    {
        LoadScene.nextLevel = "Menu";
        LoadScene.sceneEnd = true;
        ScrollModeScript.mode = null;
        ScrollModeScript.mainland = null;
    }
    /*При повторе игры необходимо обнулить количество очков, количество неправильныъ ответов, сделать панель проигрыша неактивной
    и обнулить режим и мод игры, чтобы при выходе из игры и выборе нового режима спавнились те таблички, которые нужны (иначе он запомнит эту конкретную
    игру и при следующем входе будет выдавать только выбор материка, а выбрать игру и режим не даст)*/
    public void Retry()
    {
        LoadScene.sceneEnd = true;
        points = 0;
        loseCount = 0;
        textPoints.text = "Очки: " + points;
        Start();
    }
    public void CheckRecord()
    {
        WorkWithFiles workWithRecordsFile = new WorkWithFiles();
        bool isRecord = workWithRecordsFile.checkRecord(namePlayer.text, ScrollModeScript.mode, points);
        if (isRecord)
        {
            recordPanel.SetActive(true);
        }
    }
    public void WriteRecord()
    {
        WorkWithFiles workWithRecordsFile = new WorkWithFiles();
        workWithRecordsFile.writeRecord(namePlayer.text, ScrollModeScript.mode, points.ToString());
        recordPanel.SetActive(false);
    }
}
