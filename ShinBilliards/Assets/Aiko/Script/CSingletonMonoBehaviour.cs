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
                    Debug.LogError(typeof(T) + " �̃C���X�^���X����");
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(Instance != this)
        {
            Debug.Log(typeof(T) + " �̃C���X�^���X���d�����Ă���̂Ŕj�����܂��B");
            Destroy(gameObject);
        }
    }

}
