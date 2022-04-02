using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    private void Start()
    {
        FadeController.Instance.FadeInStart();
    }

    void Update()
    {
        // エンターキーで処理実行
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FadeController.Instance.FadeOutStart();
        }

        if(FadeController.Instance.state == FadeController.FadeState.Black)
        SceneManager.LoadScene("Game");

    }
}
