using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CDebugMainBall : MonoBehaviourPunCallbacks, ITouche
{
    private Vector3 _offsetPos;

    void Start()
    {
        _offsetPos = transform.position;
    }

    public void TouchedEnter(GameObject other, Collider collider)
    {

        if (!photonView.AmOwner) return;

        // �X�e�B�b�N�ɐG���ꂽ
        CStick stick = other.GetComponent<CStick>();
        if (stick != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.y = 0.0f;
            dir.Normalize();
            GetComponent<Rigidbody>().AddForce(dir * 8.0f, ForceMode.Impulse); // ���萔
            
            CGameManager.Instance.HitBall();
        }
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
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void MoveStop()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void ChangeOwner()
    {
        photonView.RequestOwnership();
    }
}
