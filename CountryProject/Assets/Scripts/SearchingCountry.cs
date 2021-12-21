using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchingCountry : MonoBehaviour
{
    public GameObject searchPanel;
    public InputField searchInputField;
    public void OpenSearchPanel()
    {
        searchPanel.SetActive(true);
    }

    // Update is called once per frame
    public void ClickSearchButton()
    {
        ScrollModeScript.queryString = searchInputField.text;
        searchPanel.SetActive(false);
        global::LoadScene.nextLevel = "ModeSelection";
        global::LoadScene.sceneEnd = true;
    }
}
