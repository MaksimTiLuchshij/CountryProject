using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScrollModeScript : MonoBehaviour
{
    [Header("Controllers")]
    public static int panelCount;
    [Range(0, 500)]
    public int panOffset;
    [Range(0, 500)]
    public int panelSpace;
    [Range(0f, 20f)]
    public float panelSpeed;
    [Range(0f, 5f)]
    public float scaleOffset;
    [Range(0f, 20f)]
    public float zoomSpeed;
    [Header("Other objects")]
    public GameObject prefabButton;
    public GameObject prefabInfoButton;

    public ScrollRect scrollRect;

    private GameObject[] instPans;
    private Vector2[] pansPos;
    private Vector2[] pansScale;

    private RectTransform contentRect;
    private Vector2 contentVector;


    private Text text;
    private string[] mainlands;
    private string[] mods;

    private Button[] buttons;

    public static string mode;
    public static string mainland;
    public static bool isInventory;

    public GameObject searchButton;

    //Массив для определения названий

    public GameObject countriesTab;
    static int countryCount = 44;//по умолчанию открываем европу, где 44 страны
    public int selectedPanId;

    public static Sprite[] currentSprites;

    public static string tag = "eu";
    public static string queryString;

    List<Country> countries;

    public bool isScroll;

    void Start()
    {
        WorkWithFiles workWithFiles = new WorkWithFiles();
        if (mode == null)
        {
            panelCount = 3;
        }
        else if (mode != null)
            panelCount = 7;
        if (isInventory == true)
        {
            panelCount = countryCount;
            countriesTab.SetActive(true);
            searchButton.SetActive(true);
            if (string.IsNullOrEmpty(queryString))
            {
                countries = workWithFiles.getCountries(tag);
                panelCount = countries.Count;
            }
            else
            {
                countries = workWithFiles.getCountriesBySearch(queryString);
                panelCount = countries.Count;
            }
        }
        contentRect = GetComponent<RectTransform>();
        instPans = new GameObject[panelCount];
        pansPos = new Vector2[panelCount];
        pansScale = new Vector2[panelCount];
        mods = new string[3] { "Легкий", "Викторина", "Сложный" };
        mainlands = new string[7] { "Весь мир", "Европа", "Азия", "Северная Америка", "Южная Америка", "Австралия и Океания", "Африка" };
        buttons = new Button[panelCount];

        GameObject prefab;
        if (isInventory == true)
        {
            prefab = prefabInfoButton;
        }
        else
            prefab = prefabButton;

        for (int i = 0; i < panelCount; i++)
        {
            InstantiatePanelInventory(i, prefab);

        }

    }
    private void FixedUpdate()
    {
        if (contentRect.anchoredPosition.x >= pansPos[0].x && !isScroll || contentRect.anchoredPosition.x <= (pansPos[pansPos.Length - 1].x) && !isScroll)
        {
            scrollRect.inertia = false;
        }
        float nearestPos = float.MaxValue;
        for (int i = 0; i < panelCount; i++)
        {
            float distance = Mathf.Abs(contentRect.anchoredPosition.x - pansPos[i].x);
            if (distance < nearestPos)
            {
                nearestPos = distance;
                selectedPanId = i;
            }
            float scale = Mathf.Clamp(1 / (distance / panOffset) * scaleOffset, 0.5f, 1f);
            pansScale[i].x = Mathf.SmoothStep(instPans[i].transform.localScale.x, scale, panelSpeed * Time.fixedDeltaTime);
            pansScale[i].y = Mathf.SmoothStep(instPans[i].transform.localScale.x, scale, panelSpeed * Time.fixedDeltaTime);
            instPans[i].transform.localScale = pansScale[i];
        }
        float scrollVelocity = Mathf.Abs(scrollRect.velocity.x);
        if (scrollVelocity < 500 && !isScroll) scrollRect.inertia = false;
        if (isScroll || scrollVelocity > 500) return;
        contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, pansPos[selectedPanId].x, panelSpeed * Time.fixedDeltaTime);
        contentRect.anchoredPosition = contentVector;
    }
    //Подбирает текст для панелей
    private void SetText(int i, string[] headers)
    {
        text = instPans[i].GetComponentInChildren<Text>();
        text.text = headers[i];
        buttons[i] = instPans[i].GetComponentInChildren<Button>();
        //Так как кнопки - префабы, то устанавливаем всем созданным кнопкам слушателя и даем ему метод загрузки сцены
        if (isInventory != true)
        {
            buttons[i].onClick.AddListener(() => LoadScene());
            buttons[i].name = i.ToString();
        }
    }

    private void SetSprites(int i, Sprite[] sprites)
    {
        instPans[i].GetComponentsInChildren<Image>()[1].sprite = sprites[i];
    }

    private void SetSpritesByQuery(int i, Sprite[] sprites, string name)
    {
        for (int j = 0; j < 194; j++)
        {
            if (sprites[j].name.ToLower() == name.ToLower())
            {
                instPans[i].GetComponentsInChildren<Image>()[1].sprite = sprites[j];
                break;
            }
        }
    }

    public void LoadScene()
    {
        if (mode == null)
        {
            //Присваивает название кнопки режима (легкий, викторина и т.д)
            mode = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
            global::LoadScene.nextLevel = "ModeSelection";
            global::LoadScene.sceneEnd = true;
        }
        else
        {
            mainland = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
            global::LoadScene.nextLevel = "Game";
            global::LoadScene.sceneEnd = true;


        }
    }

    //Метод, который считает, сколько нужно создать панелей для конкретной части света
    public void WithdrawMainlandCountries()
    {
        tag = EventSystem.current.currentSelectedGameObject.tag;
        switch (tag)
        {
            case "eu":
                currentSprites = Resources.LoadAll<Sprite>("Countries/eu");
                countryCount = currentSprites.Length;
                break;
            case "as":
                currentSprites = Resources.LoadAll<Sprite>("Countries/as");
                countryCount = currentSprites.Length;
                break;
            case "na":
                currentSprites = Resources.LoadAll<Sprite>("Countries/na");
                countryCount = currentSprites.Length;
                break;
            case "sa":
                currentSprites = Resources.LoadAll<Sprite>("Countries/sa");
                countryCount = currentSprites.Length;
                break;
            case "au":
                currentSprites = Resources.LoadAll<Sprite>("Countries/au");
                countryCount = currentSprites.Length;
                break;
            case "af":
                currentSprites = Resources.LoadAll<Sprite>("Countries/af");
                countryCount = currentSprites.Length;
                break;
        }
        global::LoadScene.nextLevel = "ModeSelection";
        global::LoadScene.sceneEnd = true;
    }
    //С закрытием скролла все обнуляем
    public void closeScroll()
    {
        global::LoadScene.nextLevel = "Menu";
        global::LoadScene.sceneEnd = true;
        isInventory = false;
        countriesTab.SetActive(false);
        searchButton.SetActive(false);
        mode = null;

    }
    public void InstantiatePanel(int i, GameObject prefab)
    {
        //Instantiate - создает панельки с режимами игры, на вход принимает объект, который создаем - панели, родитель, в котором создаем объекты (трансформ родителя)
        //+  указываем, что будем использовать локальные координаты для создаваемых объектов (false).
        instPans[i] = Instantiate(prefab, transform, false);
        //Расставляем панели по координатам - первую оставляем на месте, остальные через пробелы
        //Расстояние считаем по формуле счетчик * (ширину панели + пробел)
        instPans[i].transform.localPosition = new Vector2(i * (prefab.GetComponent<RectTransform>().sizeDelta.x + panelSpace), 0);
        //Сохраняем позиции панелей в массиве
        pansPos[i] = -instPans[i].transform.localPosition;
    }
    public void InstantiatePanelInventory(int i, GameObject prefab)
    {
        InstantiatePanel(i, prefab);
        if (isInventory == true)
        {
            //если инвентарь true
            instPans[i].name = i.ToString();
            if (currentSprites == null)
            {
                currentSprites = Resources.LoadAll<Sprite>("Countries/eu");
            }
            countryCount = currentSprites.Length;
            instPans[i].GetComponentsInChildren<Text>()[1].text = countries[i].name;
            instPans[i].GetComponentsInChildren<Text>()[2].text = countries[i].capital;
            instPans[i].GetComponentsInChildren<Text>()[3].text = countries[i].square + " км2";
            instPans[i].GetComponentsInChildren<Text>()[4].text = countries[i].population + " чел";
            instPans[i].GetComponentsInChildren<Text>()[5].text = countries[i].currency;
            if (string.IsNullOrEmpty(queryString))
            {
                SetSprites(i, currentSprites);
            }
            else {
                currentSprites = Resources.LoadAll<Sprite>("Countries/all");
                SetSpritesByQuery(i, currentSprites, countries[i].name);
            }
        }
        else
        //Здесь на панелях взависимости от режима/сложности/материка/ выбирается текст и спрайт.
        {
            if (mode == null)
            {
                Sprite[] complexitySprites = Resources.LoadAll<Sprite>("Complexity");
                SetSprites(i, complexitySprites);
                SetText(i, mods);
            }
            else if (mode != null)
            {
                Sprite[] mainlandSprites = Resources.LoadAll<Sprite>("Mainlands");
                SetText(i, mainlands);
                SetSprites(i, mainlandSprites);
            }
        }
    }

    public void Scrolling(bool scroll)
    {
        isScroll = scroll;
        if (scroll) scrollRect.inertia = true;
    }
}



