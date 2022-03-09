using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDebugMainBall : MonoBehaviour, ITouche
{
    private Vector3 _offsetPos;

    void Start()
    {
        _offsetPos = transform.position;
    }

    public void TouchedEnter(GameObject other, Collider collider)
    {
        // �X�e�B�b�N�ɐG���ꂽ
        CStick stick = other.GetComponent<CStick>();
        if (stick != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.Normalize();
            GetComponent<Rigidbody>().AddForce(dir * 30.0f, ForceMode.Impulse); // ���萔

            stick.Destroy();
            CGameManager.Instance.HitBall(stick.GetHavePlayer());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("goal"))
        {
            // �f�����b�g

            // �Đ���
            transform.position = _offsetPos;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void MoveStop()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
