using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBall : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField]
    private float minSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(Random.Range(-10f, 10f),0f, Random.Range(-10f, 10f));
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.velocity.magnitude < minSpeed)
            _rb.velocity *= Random.Range(minSpeed, 10f/ minSpeed);
    }
}
