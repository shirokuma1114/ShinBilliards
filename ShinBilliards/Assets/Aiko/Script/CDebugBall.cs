using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CDebugBall : MonoBehaviourPunCallbacks//, ITouche
{
    [SerializeField] private int _num = 1;

    public int Get_num()
    {
        return _num;
    }

    public void TouchedEnter(GameObject other, Collider collider)
    {
        // スティックに触れられた
        CStick stick = other.GetComponent<CStick>();
        if (stick != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.Normalize();
            GetComponent<Rigidbody>().AddForce(dir * 30.0f, ForceMode.Impulse); // 仮定数

            stick.Destroy();
            CGameManager.Instance.HitBall(stick.GetHavePlayer());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("goal"))
        {
            CGameManager.Instance.GoalBall(this);
            Destroy(gameObject);
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
