using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFootSmoke : MonoBehaviour
{

    private Rigidbody _rb = null;
    [SerializeField]
    private GameObject _effect = null;

    ParticleSystem _ps;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _ps = _effect.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        var emmision = _ps.emission;
        emmision.enabled = true;
        if(_rb.velocity.magnitude > 0.05f)
        {
            emmision.rateOverTimeMultiplier = 12;

        }
        else
        {
            emmision.rateOverTimeMultiplier = 0;
        }
    }
}
