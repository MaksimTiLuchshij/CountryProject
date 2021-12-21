using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitConfirm : MonoBehaviour
{
    // Start is called before the first frame update
    public void Yes()
    {

        ScrollModeScript.mode = null;
        LoadScene.nextLevel = "Menu";
        LoadScene.sceneEnd = true;
    }

    public void No(GameObject exitConfirm)
    {
        exitConfirm.SetActive(false);
    }
}
