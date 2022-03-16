using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerMe : MonoBehaviour
{
    void Start()
    {
        transform.parent.GetComponent<Canvas>().worldCamera = Camera.main;
    }
    
}
