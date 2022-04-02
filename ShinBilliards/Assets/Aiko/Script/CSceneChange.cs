using UnityEngine;
using UnityEngine.SceneManagement;

public class CSceneChange : MonoBehaviour
{
    public void GotoScene(string name)
    {
        SceneManager.LoadScene(name);
    }

}
