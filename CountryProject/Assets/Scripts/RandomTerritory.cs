using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandomTerritory : MonoBehaviour
{
    public Image mainlandImage;
    public Image countryImage;
    public Sprite[] mainlands;
    public Sprite[] countries;
    public GameObject[] buttonAnswers;
    public GameObject losePanel;
    public Text textPoints;
    public GameObject inputField;
    int randNum;
    int points = 0;
    int loseCount = 0;
    int buttonCount = 4;
    string countryStr;
    public GameObject exitConfirm;
    System.Random random = new System.Random();
    Button activeButton;
    Color activeColor;
    // Start is called before the first frame update
    void Start()
    {
        switch (ScrollModeScript.mainland)
        {
            case "Весь мир":
                mainlandImage.GetComponent<Image>().sprite = mainlands[0];
                break;
            case "Европа":
                mainlandImage.GetComponent<Image>().sprite = mainlands[1];
                break;
            case "Азия":
                mainlandImage.GetComponent<Image>().sprite = mainlands[2];
                break;
            case "Северная Америка":
                mainlandImage.GetComponent<Image>().sprite = mainlands[3];
                break;
            case "Южная Америка":
                mainlandImage.GetComponent<Image>().sprite = mainlands[4];
                break;
            case "Австралия и Океания":
                mainlandImage.GetComponent<Image>().sprite = mainlands[5];
                break;
            case "Африка":
                mainlandImage.GetComponent<Image>().sprite = mainlands[6];
                break;
        }
        randNum = random.Next(0, countries.Length);
        Sprite country = countries[randNum];
        countryImage.GetComponent<Image>().sprite = country;
        var nums = Enumerable.Range(0, countries.Length).OrderBy(n => random.Next()).ToArray();
        if(ScrollModeScript.mode == "Легкий режим")
        {
            buttonAnswers[2].SetActive(false);
            buttonAnswers[3].SetActive(false);
            buttonCount = 2;
        }
        else if(ScrollModeScript.mode == "Сложный режим")
        {
            buttonAnswers[0].SetActive(false);
            buttonAnswers[1].SetActive(false);
            buttonAnswers[2].SetActive(false);
            buttonAnswers[3].SetActive(false);
            inputField.SetActive(true);
            activeButton = GameObject.Find("ButtonAnswer").GetComponent<Button>();
        }
        for (int i = 0;i < buttonCount; i++)
        {
            buttonAnswers[i].GetComponentInChildren<Text>().text = countries[nums[i]].name; 
        }
        randNum = random.Next(0, buttonCount);
        buttonAnswers[randNum].GetComponentInChildren<Text>().text = country.name;
        countryStr = country.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
            activeColor = Color.red;
            loseCount++;
            if (loseCount == 5)
            {
                losePanel.SetActive(true);
                ScrollModeScript.mode = null;
                ScrollModeScript.mainland = null;
            }
        }
        textPoints.text = "Очки: " + points;
    }

    public void TextAnswer()
    {
        if (inputField.GetComponent<InputField>().text.ToLower().Equals(countryStr.ToLower()))
        {
            points++;
            ColorBlock colorBlock = activeButton.colors;
            colorBlock.normalColor = Color.green;
            activeButton.colors = colorBlock;
        }
        else
        {
            points -= 2;
            ColorBlock colorBlock = activeButton.colors;
            colorBlock.normalColor = Color.green;
            activeButton.colors = colorBlock;
            loseCount++;
            if (loseCount == 5)
            {
                losePanel.SetActive(true);
                ScrollModeScript.mode = null;
                ScrollModeScript.mainland = null;
            }
        }
        textPoints.text = "Очки: " + points;
        inputField.GetComponent<InputField>().text = "";
    }

    private IEnumerator WaitSecond()
    {
        ColorBlock defaultColor = activeButton.colors;
        SetActiveButtons(false);
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
            for (int i = 0; i < buttonCount; i++)
            {
                buttonAnswers[i].GetComponent<Button>().enabled = active;
            }
    }

    private void SetButtonColor(Button button, Color newColor)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = newColor;
        button.colors = colorBlock;
    }
    public void ExitToMenu()
    {
        LoadScene.nextLevel = "Menu";
        LoadScene.sceneEnd = true;
    }
    /*При повторе игры необходимо обнулить количество очков, количество неправильныx ответов, сделать панель проигрыша неактивной
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

    public void closeRandomTerritory()
    {
        exitConfirm.SetActive(true);
    }

}
