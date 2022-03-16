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
            CGameManager.Instance.GoalBall(this);
            _isGoal = true;
            //PhotonNetwork.Destroy(gameObject);    // �܂Ƃ߂�Instantiate���Ă���Ǝv���悤�ȋ����ɂȂ�Ȃ�
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
