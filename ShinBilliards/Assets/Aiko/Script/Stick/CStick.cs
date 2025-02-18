using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public interface ITouche
{
    void TouchedEnter(GameObject other, Collider collider);
}

public class CStick : MonoBehaviourPunCallbacks
{
    private CStickManager _mgr = null;

    private Collider _collider = null;

    [SerializeField]
    private float _pull = 0.0f;
    [SerializeField]
    private float _charge = 0.0f;

    [SerializeField]
    private Vector3 _offsetPos = new Vector3(0.153f, 0.594f, 0.789f);
    [SerializeField]
    private Vector3 _offsetRot = new Vector3(-90.0f, 0.0f, -7.966f);
    [SerializeField]
    private LayerMask _hitLayerMask = 0;
    [SerializeField]
    private GameObject _effect = null;

    [SerializeField]
    private UIPosition[] _uiPos = null;
    [SerializeField]
    private ChargeUI _chargeUI = null;

    private bool _isPlaySE = false;
    

    enum State
    {
        Placed,
        Idle,
        Charge,
        Shot,
        Attack
    }

    State _state;

    public bool GetCueIsUse
    {
        get { return _state != State.Placed; }
    }


    void Start()
    {
        _collider = GetComponent<Collider>();
        _state = State.Placed;

        SoundManager.Instance.PlaySE("Cue_Appearance");

    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (_state == State.Placed) return;

        CCueHaveEffect.Instance.transform.position = new Vector3(transform.parent.position.x, 0.01f, transform.parent.position.z);

        // キューの位置を調整する
        {
            // ボールを貫通しないよう調整
            Vector3 worldOffsetPos = transform.parent.TransformPoint(_offsetPos);
            float rayRadius = 0.3f;
            float cueLength = 4.0f;
            float rayLength = cueLength + 1.0f;

            RaycastHit hit;
            Debug.DrawRay(worldOffsetPos + transform.up * 1.0f, -transform.up * rayLength, Color.red);
            if (Physics.SphereCast(worldOffsetPos + transform.up * 1.0f, rayRadius, -transform.up, out hit, rayLength, _hitLayerMask))
            {
                Vector3 pos =  worldOffsetPos + transform.up * (cueLength - hit.distance - rayRadius);
                pos = Vector3.Lerp(pos, pos + transform.up, _pull);    // チャージ分
                transform.position = pos;

                if (hit.transform.CompareTag("MainBall"))
                {
                    CDebugMainBall tocheObj = hit.transform.GetComponent<CDebugMainBall>();
                    if (tocheObj != null)
                    {
                        Vector3 dir = hit.transform.position - transform.position;
                        // 予測線表示
                        tocheObj.ShowPrediction(dir);
                        
                        // 当たり判定
                        if (_state == State.Shot)
                        {
                            if (_pull < 0.0f)
                            {// ヒット
                                tocheObj.Hit(dir, _charge * 10.0f);
                                _pull = 0.0f;
                                _charge = 0.0f;
                                _state = State.Idle;
                            }
                        }
                    }
                }
            }
            else
            {
                Vector3 pos = worldOffsetPos;
                pos = Vector3.Lerp(pos, pos + transform.up, _pull);    // チャージ分
                transform.position = pos;
            }
        }

        if (_state == State.Shot || _state == State.Attack)
        {
            _pull -= Time.deltaTime * 10.0f;
            if(_pull <= -0.5f)
            {
                _pull = 0.0f;
                _charge = 0.0f;
                _state = State.Idle;
            }
        }


        transform.localRotation = Quaternion.Euler(_offsetRot);

    }


    public void Init(CStickManager mgr)
    {
        _mgr = mgr;
    }

    // 攻撃
    public void Attack()
    {
        if (_state != State.Idle) return;

        StartCoroutine(AttackAnim());
    }
    
    // 攻撃用アニメーション
    IEnumerator AttackAnim()
    {
        // 引く
        while (_pull < 1.0f)
        {
            _pull += Time.deltaTime * 5.0f;
            yield return null;
        }
        _pull = 1.0f;
        _state = State.Attack;
    }

    // チャージ更新
    public void Charge(GameObject player, float late)
    {
        if (!(_state == State.Idle || _state == State.Charge)) return;

        _state = State.Charge;
        _pull = late;
        _chargeUI.transform.parent.gameObject.SetActive(true);
        _chargeUI.SetCharge(late, 1.0f);

        if (!_isPlaySE)
        {
            SoundManager.Instance.PlaySE("Cue_Charge", true);
            _isPlaySE = true;
        }
        //transform.localPosition = Vector3.Lerp(_offset, _offset - Vector3.forward, late);
        //transform.position = player.transform.position + (power * -(player.transform.rotation * Vector3.forward));
        //transform.localPosition = new Vector3(0.5f, 1, 0) + (power * (player.transform.rotation * new Vector3(-1, 0, 0)));
    }

    // 打つ
    public void Hit(GameObject player, float currentCharge)
    {
        if (!(_state == State.Idle || _state == State.Charge)) return;

        _state = State.Shot;
        _pull = currentCharge / 2.0f; // 仮
        _charge = _pull;
        _chargeUI.transform.parent.gameObject.SetActive(false);
        if (_isPlaySE)
        {
            SoundManager.Instance.StopSE("Cue_Charge");
            _isPlaySE = false;
        }
        //if (_collider == null) _collider = GetComponent<Collider>();
        //_collider.enabled = true;
        //transform.localPosition = _offset;

        //StartCoroutine("ShotAnim");
    }

    // キュー取得
    public void CueUse(PlayerController player)
    {
        if (_state != State.Placed) return;

        ChangeOwner();
        _state = State.Idle;

        // 位置移動
        transform.parent = player.transform;
        transform.localPosition = _offsetPos;
        transform.localRotation = Quaternion.Euler(_offsetRot);

        CCueHaveEffect.Instance.ChangeOwner();
        foreach(UIPosition ui in _uiPos)
        {
            ui.SetTarget(player.transform);
        }
    }

    // キュー取得済み
    public void Take()
    {
        if (_collider == null) _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _effect.SetActive(false);
        CCueHaveEffect.Instance.SetActive(true);
    }

    // キュー落とす
    public void CueRelease()
    {
        _state = State.Placed;
        _pull = 0.0f;
        _charge = 0.0f;

        if (photonView.IsMine)
        {
            transform.parent = null;
            transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
            transform.rotation = Quaternion.Euler(-81.702f, 0, 0);
        }

        if (_collider == null) _collider = GetComponent<Collider>();
        _collider.enabled = true;
        _effect.SetActive(true);
        CCueHaveEffect.Instance.SetActive(false);
        _chargeUI.transform.parent.gameObject.SetActive(false);
        if (_isPlaySE)
        {
            SoundManager.Instance.StopSE("Cue_Charge");
            _isPlaySE = false;
        }
    }

    // キュー消える
    public void Destroy()
    {
        CCueHaveEffect.Instance.SetActive(false);
        if (_isPlaySE)
        {
            SoundManager.Instance.StopSE("Cue_Charge");
            _isPlaySE = false;
        }

        if (!photonView.IsMine) return;

        PhotonNetwork.Destroy(gameObject);
    }

    // オーナーチェンジ
    public void ChangeOwner()
    {
        photonView.RequestOwnership();
    }


    // 未使用
    public void SetTransform(GameObject parentObj)
    {
        //this.gameObject.transform.parent = parentObj.transform;
        //this.gameObject.transform.localPosition = new Vector3(0.5f, 1, 0);// + parentObj.transform.position;
        //this.gameObject.transform.localRotation = Quaternion.Euler(0, 90, 90);// * Quaternion.Euler(parentObj.transform.forward);
    }

    // 未使用
    public void ResetTransform()
    {
        //this.gameObject.transform.parent = null;

        //this.gameObject.transform.localPosition += new Vector3(-0.1f, 0, 0);
        //this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {

        //// 拾う時の処理
        //if(_state == State.Placed)
        //{
        //    /*
        //    CPlayer touchPlayer = other.transform.GetComponent<CPlayer>();
        //    if(touchPlayer != null)
        //    {
        //        _touchPlayer = touchPlayer;
        //        transform.parent = touchPlayer.transform;
        //        _collider.enabled = false;
        //        _state = State.Idle;
        //        _offset = new Vector3(0.64f, 0.25f, 1.28f);
        //        transform.localPosition = _offset;
        //        transform.localRotation = Quaternion.Euler(-81.7f, 0.0f, 0.0f);
        //        touchPlayer.GetCue(this);
        //    }
        //    */
        //    //if (other.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        //    //{
        //    //    CGameManager.Instance.GetCue();
        //    //}
        //    return;
        //}


        //return;     // 以降コライダーを使用しなくなったので無し

        //if (!photonView.AmOwner) return;

        //ITouche tocheObj = other.transform.GetComponent<ITouche>();

        //if(tocheObj != null)
        //{
        //    tocheObj.TouchedEnter(gameObject, other);
        //}
    }

}
