/*==============================================================================
    [CPlayerMover.cs]
    ・プレイヤーの動き部分
--------------------------------------------------------------------------------
    2021.10.11 @Fujiwara Aiko
================================================================================
    History
        2021.10.11 Fujiwara Aiko
            スクリプト追加
            
/*============================================================================*/


using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CPlayerMover : MonoBehaviour
{

    [SerializeField] protected float _fWalkSpeed = 5.0f;     // 歩きのスピード
    [SerializeField] protected float _fRotateSpeed = 5.0f;   // 振り向きスピード

    protected Vector2 _vForward;      // 前方向
    protected Rigidbody _rb;          // プレイヤーのRigidbody

    //一時停止時の速度
    private Vector3 _vPauseAngularVelocity;
    private Vector3 _vPauseVelocity;

    protected virtual void Start()
    {
        // キャッシュ
        _rb = GetComponent<Rigidbody>();
        _vForward = new Vector2(transform.forward.x, transform.forward.z);
    }

    // Move 　動く
    // 引数：moveDir 動く方向, turnDir 向き
    public virtual void Move(Vector2 moveDir, Vector2 turnDir)
    {
        _rb.velocity = new Vector3(moveDir.x, 0.0f, moveDir.y).normalized * _fWalkSpeed;

        Turn(turnDir);
    }


    // Turn 回転（移動方向を向く）
    protected virtual void Turn(Vector2 dir)
    {

        //if (dir.x < -0.1f || dir.y < -0.1f ||
        //    dir.x > 0.1f || dir.y > 0.1f)
        //{// 方向が入力されていたら
        //    _vForward = dir;    // 向きを変更する
        //}

        // 向きを少しずつ変える
        //Quaternion nextRotation = Quaternion.LookRotation(new Vector3(_vForward.x, 0.0f, _vForward.y), Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, nextRotation, Time.deltaTime * _fRotateSpeed);
        if (dir.x < -0.1f)
        {
            transform.rotation *= Quaternion.AngleAxis(-Time.deltaTime * _fRotateSpeed,Vector3.up);
        }
        if (dir.x > 0.1f)
        {
            transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * _fRotateSpeed, Vector3.up);
        }
    }


    private void OnEnable()
    {
        if (_rb != null)
        {
            _rb.angularVelocity = _vPauseAngularVelocity;
            _rb.velocity = _vPauseVelocity;
        }
    }

    private void OnDisable()
    {
        if (_rb != null)
        {
            _vPauseAngularVelocity = _rb.angularVelocity;
            _vPauseVelocity = _rb.velocity;
            _rb.angularVelocity = Vector3.zero;
            _rb.velocity = Vector3.zero;
        }
    }
    
}
