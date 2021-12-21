using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorkWithFiles : MonoBehaviour
{
    public string getPath(string fileName)
    {
        string path;
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, fileName);
#else
        path = Path.Combine(Application.dataPath, fileName);
#endif
        return path;
    }
    public void writeRecord(string namePlayer, string mode, string points)
    {
        ScrolleViewRecord.TestItemModel testItemModel = new ScrolleViewRecord.TestItemModel();
        testItemModel.name = namePlayer;
        testItemModel.mode = mode;
        testItemModel.points = points;

        List<ScrolleViewRecord.TestItemModel> listRecords = new List<ScrolleViewRecord.TestItemModel>();
        listRecords = JsonConvert.DeserializeObject<List<ScrolleViewRecord.TestItemModel>>(File.ReadAllText(getPath("Records.json")));
        if (listRecords == null)
        {
            listRecords = new List<ScrolleViewRecord.TestItemModel> { testItemModel };
        }
        else
            listRecords.Add(testItemModel);
        File.WriteAllText(getPath("Records.json"), JsonConvert.SerializeObject(listRecords));
    }
    public List<ScrolleViewRecord.TestItemModel> Records()
    {
        return JsonConvert.DeserializeObject<List<ScrolleViewRecord.TestItemModel>>(File.ReadAllText(getPath("Records.json")));
    }

    public bool checkRecord(string namePlayer, string mode, int points)
    {
        bool isRecord = false;
        bool modeIsExist = false;
        List<ScrolleViewRecord.TestItemModel> listRecords = JsonConvert.DeserializeObject<List<ScrolleViewRecord.TestItemModel>>(File.ReadAllText(getPath("Records.json")));
        if (listRecords == null)
        {
            isRecord = true;
            return isRecord;
        }
       
        for (int i = 0; i < listRecords.Count; i++)
        {
            if ((int.Parse(listRecords[i].points)) < points && (listRecords[i].mode == mode))
            {
                isRecord = true;
            }
            else if (listRecords[i].mode == mode)
            {
                modeIsExist = true;
            }
        }
        if (!modeIsExist)
        {
            isRecord = true;
        }
        Debug.Log(isRecord);
        return isRecord;
    }

    public void deleteRecord(string name, string mode, string points)
    {
        List<ScrolleViewRecord.TestItemModel> listRecords = JsonConvert.DeserializeObject<List<ScrolleViewRecord.TestItemModel>>(File.ReadAllText(getPath("Records.json")));
        for (int i = 0; i < listRecords.Count; i++)
        {
            if ((listRecords[i].name == name) && (listRecords[i].mode == mode) && (listRecords[i].points == points))
            {
                listRecords.RemoveAt(i);
                File.WriteAllText(getPath("Records.json"), JsonConvert.SerializeObject(listRecords));
            }
        }
    }
    
    public List<Country> getCountries(string tag)
    {
        List<Country> countries = JsonConvert.DeserializeObject<List<Country>>(File.ReadAllText(getPath("Countries.json")));
        //Создаю второй список, чтобы забивать в него страны, которые удовлетворяют параметру "материк".
        List<Country>countries1 = new List<Country>();
        if (tag == "all" || tag == null) {
            return countries;
        }
        for (int i = 0; i < countries.Count; i++) {
            if (countries[i].mainland == tag)
            {
                countries1.Add(countries[i]);
            }
            
            
        }
        //...и возвращаю этот список стран взависимости от материка
            return countries1;
    }

    public List<Country> getCountriesBySearch(string queryString)
    {
        List<Country> countries = JsonConvert.DeserializeObject<List<Country>>(File.ReadAllText(getPath("Countries.json")));
        //Создаю второй список, чтобы забивать в него страны, которые удовлетворяют строке поиска.
        List<Country> countries1 = new List<Country>();
        for (int i = 0; i < countries.Count; i++)
        {
            if (countries[i].name.ToLower().Contains(queryString.ToLower()))
            {
                countries1.Add(countries[i]);
            }
        }
        //...и возвращаю этот список стран по строчке поиска
        return countries1;
    }
}

