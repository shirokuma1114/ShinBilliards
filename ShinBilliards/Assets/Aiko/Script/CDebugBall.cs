using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CDebugBall : MonoBehaviourPunCallbacks//, ITouche
{
    [SerializeField] private int _num = 1;
    private bool _isGoal = false;

    public int Get_num()
    {
        return _num;
    }

    public void TouchedEnter(GameObject other, Collider collider)
    {
        if (!photonView.AmOwner) return;

        // スティックに触れられた
        CStick stick = other.GetComponent<CStick>();
        if (stick != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.Normalize();
            GetComponent<Rigidbody>().AddForce(dir * 30.0f, ForceMode.Impulse); // 仮定数
            
            CGameManager.Instance.HitBall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!photonView.AmOwner) return;
        if (_isGoal) return;

        if (other.CompareTag("goal"))
        {
            CGameManager.Instance.GoalBall(this);
            _isGoal = true;
            //PhotonNetwork.Destroy(gameObject);    // まとめてInstantiateしていると思うような挙動にならない
        }
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
