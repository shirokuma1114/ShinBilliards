/*==============================================================================
    [CPlayerMoverTP.cs]
    �E�J�����O�����ɐi�ރv���C���[
--------------------------------------------------------------------------------
    2021.10.13 @Fujiwara Aiko
================================================================================
    History
        2021.10.13 Fujiwara Aiko
            �X�N���v�g�ǉ�
            
/*============================================================================*/


using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CPlayerMoverTP : CPlayerMover
{
    [SerializeField] private Transform _tPlayerLook = null;     // �v���C���[�̌����ڕ����i�i�s��������������j
    [SerializeField] private Transform _tCamera = null;          // �v���C���[�̃J�����i�i�s�����ɂȂ�j

    protected override void Start()
    {
        base.Start();
    }

    // Move �@����
    // �����FmoveDir ��������, turnDir ����
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

    // Turn ��]�i�ړ������������j
    protected override void Turn(Vector2 dir)
    {
        if (dir.x < -0.1f || dir.y < -0.1f ||
            dir.x > 0.1f || dir.y > 0.1f)
        {// ���������͂���Ă�����
            _vForward = dir;    // ������ύX����
        }

        // �������������ς���
        Quaternion nextRotation = Quaternion.LookRotation(new Vector3(_vForward.x, 0.0f, _vForward.y), Vector3.up);
        _tPlayerLook.rotation = Quaternion.RotateTowards(_tPlayerLook.rotation, nextRotation, Time.deltaTime * _fRotateSpeed);
    }
    
}
