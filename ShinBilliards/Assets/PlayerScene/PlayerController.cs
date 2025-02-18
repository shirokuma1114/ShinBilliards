using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private float hTrans;
    private float vTrans;
    private const float transSpeed = 10f;

    [SerializeField] private bool attackRange = false;
    private bool attack = false;
    private const float attackCoolTime = 0.2f;
    private float attackCurrentTime = 0;

    [SerializeField] private bool cueUse = false;

    private const float attackCuePoint = 20;
    private const float attackPoint = 10;
    public float hp = 100;

    private bool down = false;
    private bool getup = false;
    private float downCurrentTime = 0;
    private const float downTime = 5;//フレーム/60
    private const float getupTime = 3;//立ち上がり始める時間

    private GameObject otherPlayer;

    private GameObject cueObject;

    //private GameObject master;

    //[SerializeField] KeyCode upKey = KeyCode.UpArrow;
    //[SerializeField] KeyCode downKey = KeyCode.DownArrow;
    //[SerializeField] KeyCode rightKey = KeyCode.RightArrow;
    //[SerializeField] KeyCode leftKey = KeyCode.LeftArrow;
    //[SerializeField] KeyCode attackKey = KeyCode.Return;

    private string PlayerName;

    [SerializeField]private Vector3 tableCameraPos = new Vector3(0, 10.0f, 0);
    [SerializeField]private Vector3 tableCameraRot = new Vector3(90, 0, 0);
    
    [SerializeField]private Vector3 stageCameraPos = new Vector3(0, 10.0f, -10.5f);
    [SerializeField] private Vector3 stageCameraRot = new Vector3(45, 0, 0);

    [SerializeField] private float tableFieldofView = 60;
    [SerializeField] private float stageFieldofView = 70;

    private Vector3 startPos;
    private Vector3 endPos;

    private Vector3 startRot;
    private Vector3 endRot;

    private float startFieldofView;
    private float endFieldofView;

    private float cameraDistance;

    private bool cameraIn = false;

    private float cameraTransTime = 1.0f;
    private float cameraTransCurrentTime;

    bool cameraTransState;


    private bool chargeStart = false;
    private float maxCharge = 2;
    private float currentCharge = 0;
    private Rigidbody _rb;

    Animator animator;

    private Camera mainCamera;

    private CPlayerScore _score = null;
    public CPlayerScore Score
    {
        get { return _score; }
    }


    [SerializeField] private GameObject piyopiyo;

    private void Awake()
    {
        //cueObject = GameObject.FindGameObjectWithTag("Cue");
        //master = GameObject.Find("Master");

        mainCamera = Camera.main;
        mainCamera.transform.position = stageCameraPos;

        cameraDistance = Vector3.Distance(stageCameraPos, tableCameraPos);
        animator = gameObject.GetComponent<Animator>();

        _score = GetComponent<CPlayerScore>();

        _rb = GetComponent<Rigidbody>();

        piyopiyo.SetActive(false);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        InputUpdate();
        TransUpdate();
        CameraUpdate();
        AttackUpdate();
        DamageUpdate();
        BilliardPlayUpdate();
    }

    void InputUpdate()
    {
        if (!photonView.IsMine) return;


        hTrans = vTrans = 0;

        if (!Input.anyKey) return;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            vTrans = 1;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            vTrans = -1;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            hTrans = 1;
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            hTrans = -1;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            attack = true;
    }

    void TransUpdate()
    {
        if (down) return;

        Vector3 pos = new Vector3(hTrans, 0, vTrans);
        _rb.velocity = Vector3.Normalize(pos) * transSpeed;
        //pos = Vector3.Normalize(pos) * transSpeed * Time.deltaTime;
        //transform.position += pos;

        if(pos != new Vector3(0,0,0))
        {
            animator.SetBool("Run",true);
            transform.rotation = Quaternion.LookRotation(pos);

        }
        else
        {
            animator.SetBool("Run", false);
            _rb.angularVelocity = Vector3.zero;
        }


    }

    void AttackUpdate()
    {
        if (!attack) return;//攻撃入力取得
        if (!photonView.IsMine) return;


        bool otherDown = false;

        if (otherPlayer != null)
            otherDown = otherPlayer.GetComponent<PlayerController>().down;//敵がダウン中の攻撃判定



        if (attackCurrentTime == 0)//開始フレームで攻撃
        {
            //敵にダメージを与える
            if (cueUse)
            {
                animator.SetTrigger("CueAttack");
                CGameManager.Instance._cueManager.Cue().Attack();
                if (attackRange && !otherDown)//敵が攻撃範囲
                    photonView.RPC(nameof(CueAttackRPC), RpcTarget.All);
            }
            else
            {
                animator.SetTrigger("Attack");

                if (attackRange && !otherDown)//敵が攻撃範囲
                    photonView.RPC(nameof(AttackRPC), RpcTarget.All);
            }
        }

        attackCurrentTime += Time.deltaTime;

        //クールタイム
        if(attackCurrentTime>= attackCoolTime)
        {
            attackCurrentTime = 0;
            attack = false;
        }
    }

    private void CameraUpdate()
    {
        if (!photonView.IsMine) return;


        if (cameraTransState)
        {
            float speed = 5f;

            cameraTransCurrentTime += (Time.deltaTime * speed) / cameraTransTime;



            if (cameraTransCurrentTime >= 1.0f)
            {
                cameraTransState = false;
                cameraTransCurrentTime = 0;
                return;
            }

            Vector3 transPos = Vector3.Lerp(startPos, endPos, cameraTransCurrentTime);
            Vector3 transRot = Vector3.Slerp(startRot, endRot, cameraTransCurrentTime);
            float view = endFieldofView * cameraTransCurrentTime + startFieldofView * (1 - cameraTransCurrentTime);

            mainCamera.transform.position = transPos;
            mainCamera.transform.rotation = Quaternion.Euler(transRot);
            mainCamera.fieldOfView = view;

        }
    }

    private void DamageUpdate()
    {
        if (!photonView.IsMine) return;

        if (hp <= 0 && !down)
        {
            // 落とす
            if (cueUse)
            {
                CGameManager.Instance.LostCue();
            }
            down = true;
            cueUse = false;
            hp = 100;

            animator.SetBool("Knockdown",true);
            animator.SetBool("GetUp",false);
            _rb.isKinematic = true;

            StartCoroutine("CreatePiyo");
        }
        else if (down)
        {
            downCurrentTime += Time.deltaTime;

            if(downCurrentTime >= getupTime && !getup)
            {
                animator.SetBool("Knockdown", false);
                animator.SetBool("GetUp", true);

                photonView.RPC(nameof(FlashDown), RpcTarget.All);


                getup = true;
            }
            else if (downCurrentTime >= downTime)
            {
                down = false;
                downCurrentTime = 0;
                getup = false;
                animator.SetBool("GetUp", false);
                _rb.isKinematic = false;

            }
        }
        
    }


    private void BilliardPlayUpdate()
    {
        if (!cameraIn) return;
        if (!photonView.IsMine) return;
        if (down) return;
        if (!cueUse) return;

        //プレイヤー向き
        var targetWorldPos = this.gameObject.transform.position;

        // ワールド座標をスクリーン座標に変換
        Vector2 targetScreenPos = mainCamera.WorldToScreenPoint(targetWorldPos);
        Vector2 mousePos = Input.mousePosition;

        Vector2 dir2d = mousePos - targetScreenPos;
        Vector3 dir3d = new Vector3(dir2d.x, 0, dir2d.y);
        dir3d = Vector3.Normalize(dir3d);

        transform.rotation = Quaternion.LookRotation(dir3d);


        //マウスクリックでチャージ
        if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Return))
        {
            chargeStart = true;
            animator.SetBool("Shot", true);
        }
        else if(Input.GetMouseButton(1) || Input.GetKey(KeyCode.Return))
        {
            
            if(currentCharge >= maxCharge)
            {
                //突く
                
                //CGameManager.Instance._cueManager.Cue().Hit(this.gameObject, currentCharge);
                animator.SetBool("Shot", false);
                //chargeStart = false;
                //currentCharge = 0;
                return;
            }

            currentCharge += Time.deltaTime;// * 0.1f;

            CGameManager.Instance._cueManager.Cue().Charge(this.gameObject, currentCharge / maxCharge);

        }
        else if(Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Return))
        {
            //突く
            chargeStart = false;
            CGameManager.Instance._cueManager.Cue().Hit(this.gameObject, currentCharge);
            animator.SetBool("Shot", false);
            currentCharge = 0;

        }



    }

    [PunRPC]
    private void FlashDown()
    {
        bool active = piyopiyo.activeSelf;

        piyopiyo.SetActive(!active);
    }

    IEnumerator CreatePiyo()
    {
        yield return new WaitForSeconds(0.5f);

        photonView.RPC(nameof(FlashDown), RpcTarget.All);

        yield return null;
    }


    [PunRPC]
    private void AttackRPC()
    {
        if(otherPlayer != null)
            otherPlayer.GetComponent<PlayerController>().hp -= attackPoint;
    }

    [PunRPC]
    private void CueAttackRPC()
    {
        if (otherPlayer != null)
            otherPlayer.GetComponent<PlayerController>().hp -= attackCuePoint;
    }

    [PunRPC]
    private void CueUse(PhotonMessageInfo info)
    {
        if(PhotonNetwork.NetworkingClient.LocalPlayer.ActorNumber == info.Sender.ActorNumber)
        {
            cueObject.GetComponent<CStick>().ChangeOwner();
        }

        //cueObject.GetComponent<CStick>().CueUse();
        cueObject.GetComponent<CStick>().SetTransform(this.gameObject);
    }
    
    public void CueUse()
    {
        cueUse = true;
    }

    [PunRPC]
    private void CueRelease(PhotonMessageInfo info)
    {
        cueObject.GetComponent<CStick>().CueRelease();
        cueObject.GetComponent<CStick>().ResetTransform();
    }
    
    public void CueNoUse()
    {
        cueUse = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            attackRange = true;
            otherPlayer = other.gameObject;
        }

        if (other.gameObject.tag == "Cue")
        {
            if (!other.GetComponent<CStick>().GetCueIsUse && !this.down)
            {
                //cueUse = true;
                CGameManager.Instance.GetCue();
                //photonView.RPC(nameof(CueUse), RpcTarget.All);
            }
        }

        //カメラ遷移
        if (other.gameObject.name == "CameraTransCollider")
        {
            startPos = mainCamera.transform.position;
            endPos = tableCameraPos;

            startRot = mainCamera.transform.rotation.eulerAngles;
            endRot = tableCameraRot;

            startFieldofView = mainCamera.fieldOfView;
            endFieldofView = tableFieldofView;

            cameraTransState = true;
            cameraTransCurrentTime = 0;

            cameraIn = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            attackRange = true;
            otherPlayer = other.gameObject;
        }

        if(other.gameObject.tag == "Cue")
        {
            if(!other.GetComponent<CStick>().GetCueIsUse && !this.down)
            {
                //cueUse = true;
                CGameManager.Instance.GetCue();
                //photonView.RPC(nameof(CueUse), RpcTarget.All);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            attackRange = false;

        if (other.gameObject.tag == "Cue")
            cueUse = false;


        //カメラ遷移
        if (other.gameObject.name == "CameraTransCollider")
        {
            startPos = mainCamera.transform.position;
            endPos = stageCameraPos;

            startRot = mainCamera.transform.rotation.eulerAngles;
            endRot = stageCameraRot;

            startFieldofView = mainCamera.fieldOfView;
            endFieldofView = stageFieldofView;


            cameraTransState = true;
            cameraTransCurrentTime = 0;

            cameraIn = false;

        }
    }

}
