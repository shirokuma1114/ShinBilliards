using UnityEngine.SceneManagement;
using UnityEngine;

public class CSceneChange : CSingletonMonoBehaviour<CSceneChange>
{
    private string _nextSceneName = "";
    private bool _isSceneChange = false;
    public void GotoScene(string name)
    {
        if (!_isSceneChange)
        {
            Time.timeScale = 1.0f;
            _nextSceneName = name;
            _isSceneChange = true;
            FadeController.Instance.FadeOutStart();
        }
    }

    private void Update()
    {
        if (_isSceneChange)
        {
            if (FadeController.Instance.state == FadeController.FadeState.Black)
            {
                SceneManager.LoadScene(_nextSceneName);
            }
        }
    }

}
