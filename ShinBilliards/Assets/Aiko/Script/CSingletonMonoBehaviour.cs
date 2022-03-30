using UnityEngine;

public abstract class CSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if(_instance == null)
                {
                    Debug.LogError(typeof(T) + " のインスタンス無し");
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(Instance != this)
        {
            Debug.Log(typeof(T) + " のインスタンスが重複しているので破棄します。");
            Destroy(gameObject);
        }
    }

}
