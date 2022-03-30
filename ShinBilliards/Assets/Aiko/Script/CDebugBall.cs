using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CDebugBall : MonoBehaviourPunCallbacks//, ITouche
{
    [SerializeField] private int _num = 1;
    private bool _isGoal = false;
    private bool _isStart = false;  // �r�����[�h���X�^�[�g���Ă��邩


    public int Get_num()
    {
        return _num;
    }

    // ���g�p
    public void TouchedEnter(GameObject other, Collider collider)
    {
        if (!photonView.AmOwner) return;

        // �X�e�B�b�N�ɐG���ꂽ
        CStick stick = other.GetComponent<CStick>();
        if (stick != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.Normalize();
            GetComponent<Rigidbody>().AddForce(dir * 30.0f, ForceMode.Impulse); // ���萔
            
            CGameManager.Instance.HitBall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!photonView.AmOwner) return;
        if (_isGoal) return;

        if (other.CompareTag("goal"))
        {
            CGameManager.Instance.GoalBall(this, other.bounds.center);
            _isGoal = true;
            //PhotonNetwork.Destroy(gameObject);    // �܂Ƃ߂�Instantiate���Ă���Ǝv���悤�ȋ����ɂȂ�Ȃ�
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isStart)
        {
            // �Փ˗͂ɂ���ĉ��ʕω�������
            float force = collision.impulse.magnitude;
            if (force < 0.1f) return;
            force = force * 0.25f;
            if (force > 1.0f) force = 1.0f;

            if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
            {
                if (photonView.AmOwner)
                {
                    photonView.RPC(nameof(Ball_CollideSoundRPC), RpcTarget.All, force);
                }
                //SoundManager.Instance.PlaySE("Ball_Collide", false, force);
            }
            else
            {
                SoundManager.Instance.PlaySE("Cue_Shot", false, force);
            }
        }
    }

    // �T�E���h��RPC�Ŗ炷
    [PunRPC]
    private void Ball_CollideSoundRPC(float force)
    {
        SoundManager.Instance.PlaySE("Ball_Collide", false, force);
    }


    public void MoveStop()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void ChangeOwner()
    {
        photonView.RequestOwnership();
    }


    public void StartBilliards()
    {
        _isStart = true;
    }
}
