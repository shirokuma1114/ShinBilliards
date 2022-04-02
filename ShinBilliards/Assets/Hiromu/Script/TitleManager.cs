using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] FadeController fadeController;

    void Update()
    {
        // エンターキーで処理実行
        if (Input.GetKeyDown(KeyCode.Return))
        {
            fadeController.FadeOutStart();
        }

        if(fadeController.state == FadeController.FadeState.Black)
        SceneManager.LoadScene("Game");

    }
}
