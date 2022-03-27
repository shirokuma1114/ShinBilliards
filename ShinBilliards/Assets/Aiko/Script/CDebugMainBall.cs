using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CDebugMainBall : MonoBehaviourPunCallbacks, ITouche
{
    private Vector3 _offsetPos;
    private bool _isStart = false;  // �r�����[�h���X�^�[�g���Ă��邩

    CPredictionLine _predictionLine = null;

    void Start()
    {
        _offsetPos = transform.position;

        _predictionLine = GetComponent<CPredictionLine>();
    }

    public void TouchedEnter(GameObject other, Collider collider)
    {

        if (!photonView.AmOwner) return;

        //// �X�e�B�b�N�ɐG���ꂽ
        //CStick stick = other.GetComponent<CStick>();
        //if (stick != null)
        //{
        //    Vector3 dir = transform.position - other.transform.position;
        //    dir.y = 0.0f;
        //    dir.Normalize();
        //    GetComponent<Rigidbody>().AddForce(dir * 8.0f, ForceMode.Impulse); // ���萔
            
        //    CGameManager.Instance.HitBall();
        //}
    }

    public void ShowPrediction(Vector3 dir)
    {
        dir.y = 0.0f;
        dir.Normalize();
        _predictionLine.Show(dir);
    }

    public void Hit(Vector3 dir, float force)
    {
        dir.y = 0.0f;
        dir.Normalize();
        GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.Impulse);

        CGameManager.Instance.HitBall();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!photonView.AmOwner) return;

        if (other.CompareTag("goal"))
        {
            // �f�����b�g

            // �Đ���
            photonView.RPC(nameof(ReCreate), RpcTarget.All);
        }
    }

    [PunRPC]
    public void ReCreate()
    {
        // �Đ���
        transform.position = _offsetPos;
        MoveStop();
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
                SoundManager.Instance.PlaySE("Ball_Collide", false, force);
            }
            else
            {
                SoundManager.Instance.PlaySE("Cue_Shot", false, force);
            }
        }
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
