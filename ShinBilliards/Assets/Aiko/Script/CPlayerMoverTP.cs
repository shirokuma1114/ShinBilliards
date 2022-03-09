/*==============================================================================
    [CPlayerMoverTP.cs]
    ・カメラ前方向に進むプレイヤー
--------------------------------------------------------------------------------
    2021.10.13 @Fujiwara Aiko
================================================================================
    History
        2021.10.13 Fujiwara Aiko
            スクリプト追加
            
/*============================================================================*/


using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CPlayerMoverTP : CPlayerMover
{
    [SerializeField] private Transform _tPlayerLook = null;     // プレイヤーの見た目部分（進行方向を向かせる）
    [SerializeField] private Transform _tCamera = null;          // プレイヤーのカメラ（進行方向になる）

    protected override void Start()
    {
        base.Start();
    }

    // Move 　動く
    // 引数：moveDir 動く方向, turnDir 向き
    public override void Move(Vector2 moveDir, Vector2 turnDir)
    {
        Vector3 forward = new Vector3(_tCamera.forward.x, 0.0f, _tCamera.forward.z);
        forward.Normalize();
        Vector3 right = new Vector3(_tCamera.right.x, 0.0f, _tCamera.right.z);
        forward.Normalize();
        Vector3 direction = forward * moveDir.y + right * moveDir.x;
        direction.Normalize();
        
        _rb.velocity = direction * _fWalkSpeed;

        Turn(new Vector2(direction.x, direction.z));

    }

    // Turn 回転（移動方向を向く）
    protected override void Turn(Vector2 dir)
    {
        if (dir.x < -0.1f || dir.y < -0.1f ||
            dir.x > 0.1f || dir.y > 0.1f)
        {// 方向が入力されていたら
            _vForward = dir;    // 向きを変更する
        }

        // 向きを少しずつ変える
        Quaternion nextRotation = Quaternion.LookRotation(new Vector3(_vForward.x, 0.0f, _vForward.y), Vector3.up);
        _tPlayerLook.rotation = Quaternion.RotateTowards(_tPlayerLook.rotation, nextRotation, Time.deltaTime * _fRotateSpeed);
    }
    
}
