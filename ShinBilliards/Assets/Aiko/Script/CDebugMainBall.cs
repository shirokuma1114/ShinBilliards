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
            // デメリット

            // 再生成
            transform.position = _offsetPos;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void MoveStop()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
