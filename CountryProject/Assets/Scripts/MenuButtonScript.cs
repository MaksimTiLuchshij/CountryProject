using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonScript : MonoBehaviour
{
    public InputField inputField;
    //Затычка, чтобы было откуда считывать json текст. По нормальному нужно узнать, как сохранить файл с записями в сборке
    public Text countryJsonText;
    // Start is called before the first frame update
    void Start()
    {
        WorkWithFiles workWithRecordsFile = new WorkWithFiles();
        if (!File.Exists(workWithRecordsFile.getPath("Records.json")))
        {
            File.WriteAllText(workWithRecordsFile.getPath("Records.json"), " ");
        }
        if (!File.Exists(workWithRecordsFile.getPath("Countries.json")))
        {
            File.WriteAllText(workWithRecordsFile.getPath("Countries.json"), countryJsonText.text);
        }
    }
    
    public void ButtonPlay()
    {
        //благодаря этому коду сцена плавно затемняется и затем происходит переход на сцену указанную в nextlevel
        ScrollModeScript.isInventory = false;
        LoadScene.nextLevel = "ModeSelection";
        LoadScene.sceneEnd = true;
    }

    public void ButtonCountries()
    {
        ScrollModeScript.isInventory = true;
        //благодаря этому коду сцена плавно затемняется и затем происходит переход на сцену указанную в nextlevel
        LoadScene.nextLevel = "ModeSelection";
        LoadScene.sceneEnd = true;
    }

    public void ButtonRecords()
    {
        ScrollModeScript.isInventory = true;
        //благодаря этому коду сцена плавно затемняется и затем происходит переход на сцену указанную в nextlevel
        LoadScene.nextLevel = "Records";
        LoadScene.sceneEnd = true;
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }


}
