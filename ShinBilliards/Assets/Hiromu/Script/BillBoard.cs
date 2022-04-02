using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void Start()
    {
        target = Camera.main.transform;
    }

    void Update()
    {
        if(gameObject.activeSelf)
        {
            Vector3 pos = target.position;
            pos.y = transform.position.y;
            transform.LookAt(pos);
        }
    }
}