using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDebugBall : MonoBehaviour, ITouche
{
    public void TouchedEnter(GameObject other, Collider collider)
    {
        // �X�e�B�b�N�ɐG���ꂽ
        CStick stick = other.GetComponent<CStick>();
        if (stick != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.Normalize();
            GetComponent<Rigidbody>().AddForce(dir * 30.0f, ForceMode.Impulse); // ���萔
        }
    }
}
