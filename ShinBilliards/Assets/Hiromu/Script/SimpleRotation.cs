using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField]
    private float speed=50f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime* speed);
    }
}
