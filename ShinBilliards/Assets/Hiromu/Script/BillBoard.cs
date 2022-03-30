using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    void Update()
    {
        Vector3 pos = target.position;
        pos.y = transform.position.y;
        transform.LookAt(pos);
    }
}