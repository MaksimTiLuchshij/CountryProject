using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScrolleViewRecord : MonoBehaviour
{
    public RectTransform prefab;
    public RectTransform content;
    private List<TestItemModel> recordNotes;

    private void Start()
    {
        WorkWithFiles workWithRecordsFile = new WorkWithFiles();
        recordNotes = workWithRecordsFile.Records();
        UpdateItems();
    }
    public void UpdateItems()
    {
        //� ����� ��������� ������� ������ ������, �������, ����� �� ������� ������ �������, ����� ���� ������ ����� ����� ����� ����������
        int modelCount = recordNotes.Count;
        StartCoroutine(GetItems(modelCount, results => OnReceivedModels(results)));
    }

    IEnumerator GetItems(int count, System.Action<TestItemModel[]> callback)
    {
        yield return new WaitForSeconds(0f);
        var results = new TestItemModel[count];

        for (int i = 0; i < count; i++)
        {
            results[i] = new TestItemModel();
            results[i].name = recordNotes[i].name;
            results[i].mode = recordNotes[i].mode;
            results[i].points = recordNotes[i].points;
        }
        callback(results);
    }

    //������� ��� ��������� ����������
    void OnReceivedModels(TestItemModel[] models)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        foreach (var model in models)
        {
            var instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
            instance.transform.SetParent(content, false);
            InitializeItemView(instance, model);
        }
    }
    //������� ��� ����� ���y������� ���������� � ��
    void InitializeItemView(GameObject viewGameObject, TestItemModel model)
    {
        TestItemView view = new TestItemView(viewGameObject.transform);
        view.name.text = model.name;
        view.mode.text = model.mode;
        Debug.Log(model.mode);
        view.points.text = model.points;
        view.clickButton.onClick.AddListener(() =>
        {
            Destroy(viewGameObject);
            WorkWithFiles workWithRecordsFile = new WorkWithFiles();
            workWithRecordsFile.deleteRecord(view.name.text, view.mode.text, view.points.text);
        });
    }
    [Serializable]
    public class TestItemModel
    {
        public string name;
        public string mode;
        public string points;
    }

    public class TestItemView
    {
        public Text name;
        public Text mode;
        public Text points;
        public Button clickButton;
        public TestItemView(Transform rootView)
        {
            name = rootView.Find("Name").GetComponent<Text>();
            mode = rootView.Find("Mode").GetComponent<Text>();
            points = rootView.Find("Points").GetComponent<Text>();
            clickButton = rootView.Find("Delete").GetComponent<Button>();
        }
    }
    public void CloseRecords()
    {
        global::LoadScene.nextLevel = "Menu";
        global::LoadScene.sceneEnd = true;
    }
}
