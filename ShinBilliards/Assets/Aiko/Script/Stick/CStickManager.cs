
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class CStickManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private CStick _cuePrefab = null;
    [SerializeField] private bool _isCollectGeneratingPoints = true;
    [SerializeField] private Transform[] _generatingPoints = null;

    private CStick _cue = null;

    public CStick Cue()
    {
        return _cue;
    }
    
    public void Init()
    {
        // 生成ポイント収集(_generatingPointsの元の内容は使用しない)
        if(_isCollectGeneratingPoints)
        {
            Transform pointsParent = transform.Find("GeneratingPoints");
            if (pointsParent == null) return;
            
            _generatingPoints = 
                pointsParent.GetComponentsInChildren<Transform>()
                .Where(c => pointsParent.gameObject != c.gameObject)
                .ToArray();

        }
    }


    // 基本マスタークライアントのみ
    // キューを作成
    // 引数　force : 強制的に作成するか(既存のが消える)
    public void CreateCue(bool force)
    {

        if (_cuePrefab == null)
        {
            Debug.LogError("キュープレハブ無し");
            return;
        }
        if (_generatingPoints == null || _generatingPoints.Length <= 0)
        {
            Debug.LogError("キュー生成位置指定なし");
            return;
        }

        if (_cue != null)
        {
            if (!force) return;     // キューがまだ存在する場合新しく作成しない

            Destroy(_cue.gameObject);        // 強制作成の場合以前のものを破棄する
            _cue = null;
        }

        // ランダム位置に新しく配置
        Transform point = _generatingPoints[Random.Range(0, _generatingPoints.Length)];
        GameObject cue = PhotonNetwork.Instantiate("PFB_Stick", point.position, point.rotation, 0);
        _cue = cue.GetComponent<CStick>();
        photonView.RPC(nameof(SetCueRPC), RpcTarget.Others);
        //_cue = Instantiate(_cuePrefab, point.position, point.rotation);
        _cue.Init(this);

    }

    // 他クライアントにセット
    [PunRPC]
    private void SetCueRPC()
    {
        _cue = FindObjectOfType<CStick>();
    }


    // キューがDestroyされたときに呼び出す
    public void OnDestroyCue(CStick cue)
    {
        if(_cue == cue)
        {
            _cue = null;
        }
    }

}
