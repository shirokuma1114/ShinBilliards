using UnityEngine;
using Photon.Pun;

public class CCueHaveEffect : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _effect;

    private static CCueHaveEffect _instance;
    public static CCueHaveEffect Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CCueHaveEffect>();
                if (_instance == null)
                {
                    Debug.LogError("CCueHaveEffect無し");
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void SetActive(bool active)
    {
        _effect.SetActive(active);
    }


    // オーナーチェンジ
    public void ChangeOwner()
    {
        photonView.RequestOwnership();
    }

}
