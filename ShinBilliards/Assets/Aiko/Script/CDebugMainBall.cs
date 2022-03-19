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

        // スティックに触れられた
        CStick stick = other.GetComponent<CStick>();
        if (stick != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.y = 0.0f;
            dir.Normalize();
            GetComponent<Rigidbody>().AddForce(dir * 8.0f, ForceMode.Impulse); // 仮定数
            
            CGameManager.Instance.HitBall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!photonView.AmOwner) return;

        if (other.CompareTag("goal"))
        {
            // デメリット

            // 再生成
            photonView.RPC(nameof(ReCreate), RpcTarget.All);
        }
    }

    [PunRPC]
    public void ReCreate()
    {
        // 再生成
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
